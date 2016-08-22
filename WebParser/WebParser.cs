using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace WebParser
{
    public class WebParser
    {
        #region Variables

        /// <summary>
        /// Thread for the loading of the website source code
        /// and parsing the source code
        /// </summary>
        private Thread _threadWebParser;

        /// <summary>
        /// This flag starts the thread
        /// </summary>
        private bool _threadRunning = false;

        /// <summary>
        /// This object is used for the thread lock
        /// </summary>
        private Object thisLockStarting = new Object();
        private Object thisLockThread = new Object();

        /// <summary>
        /// String for the website url which should be parsed
        /// </summary>
        private string _webSite = "invalid";

        /// <summary>
        /// User agent identifier with the default value
        /// </summary>
        private string _userAgentIdentifier = @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0";

        /// <summary>
        /// Encoding type for the download content
        /// </summary>
        private string _encodingType = Encoding.Default.ToString();

        /// <summary>
        /// String with the loaded website content
        /// </summary>
        private string _webSiteContent;

        /// <summary>
        /// Byte array with the downloaded data content
        /// </summary>
        private byte[] _dataContent;

        /// <summary>
        /// Flag if the download of the website content is complete
        /// </summary>
        private bool _downloadComplete = false;

        /// <summary>
        /// Status of the webparser
        /// </summary>
        private WebParserState _state = WebParserState.Idle;

        /// <summary>
        /// Status of the webparser as percent
        /// </summary>
        private int _percent = 0;

        /// <summary>
        /// Last error code
        /// </summary>
        private WebParserErrorCodes _lastErrorCode = WebParserErrorCodes.NoError;

        /// <summary>
        /// List for the regex string for the parsing
        /// </summary>
        private RegExList _regexList = null;

        /// <summary>
        /// Value of the last parsed regex key
        /// </summary>
        private string _lastRegexListKey = "";

        /// <summary>
        /// Dictionary with the search result
        /// Key is the regex string
        /// Value is the search result of the regex
        /// </summary>
        private Dictionary<string, List<string>> _searchResult = null;

        /// <summary>
        /// Stores the last throw exception
        /// </summary>
        private Exception _lastException = null;

        /// <summary>
        /// Current state of the WebParser
        /// </summary>
        private WebParserInfoState _webParserInfoState = new WebParserInfoState();

        /// <summary>
        /// Flag if the thread should be canceled
        /// </summary>
        private bool _cancelThread;

        #endregion Variables

        #region Properties

        public bool ThreadRunning
        {
            get
            {
                return _threadRunning;
            }

            internal set
            {
                _threadRunning = value;
            }
        }

        public string UserAgentIdentifier
        {
            get { return _userAgentIdentifier; }
            set 
            { 
                _userAgentIdentifier = value;
                _webParserInfoState.UserAgentIdentifier = value;
            }
        }

        public string EncodingType
        {
            get { return _encodingType; }
            set
            {
                _encodingType = value;
            }
        }

        public string WebSiteContent
        {
            get { return _webSiteContent; }
            internal set { _webSiteContent = value; }
        }

        public byte[] DataContent
        {
            get { return _dataContent; }
            internal set { _dataContent = value; }
        }

        public bool DownloadComplete
        {
            get { return _downloadComplete; }
            internal set { _downloadComplete = value; }
        }

        public string WebSite
        {
            get { return _webSite; }
            set
            {
                try
                {
                    // Check if the website url is valid
                    //                String regexPattern = @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
                    //                if (System.Text.RegularExpressions.Regex.IsMatch(value, regexPattern))
                    if (value != "")
                    {
                        Uri uriWebSite = new Uri(value);
                        UriHostNameType uriHostNameType = uriWebSite.HostNameType;
                        string strDNSName = uriWebSite.DnsSafeHost;
                        string strWebSiteScheme = uriWebSite.Scheme;
                        bool isWllFormedUriStringFlag = Uri.IsWellFormedUriString(value, UriKind.Absolute);
                        if (isWllFormedUriStringFlag &&
                            (strWebSiteScheme == Uri.UriSchemeHttp || strWebSiteScheme == Uri.UriSchemeHttps) &&
                            (uriHostNameType == UriHostNameType.Dns || uriHostNameType == UriHostNameType.IPv4 || uriHostNameType == UriHostNameType.IPv6)
                            )
                        {
                            if (!Regex.IsMatch(value, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
                                _webSite = @"invalid";
                            else
                                _webSite = value;
                        }
                        else
                            _webSite = @"invalid";
                    }
                    else
                        _webSite = @"invalid";

                    _webParserInfoState.WebSite = _webSite;
                }
                catch
                {
                    _webSite = @"invalid";
                }
            }
        }

        public WebParserErrorCodes LastErrorCode
        {
            get { return _lastErrorCode; }
            internal set
            {
                _lastErrorCode = value;
                _webParserInfoState.LastErrorCode = value;
            }
        }

        public WebParserState State
        {
            get { return _state; }
            internal set
            {
                _state = value;
                _webParserInfoState.State = value;
            }
        }

        public int Percent
        {
            get { return _percent; }
            internal set
            {
                _percent = value;
                _webParserInfoState.Percentage = value;
            }
        }

        public RegExList RegexList
        {
            get { return _regexList; }
            set
            {
                _regexList = value;
                _webParserInfoState.RegexList = value;
            }
        }

        public string LastRegexListKey
        {
            get { return _lastRegexListKey; }
            internal set
            {
                _lastRegexListKey = value;
                _webParserInfoState.LastRegexListKey = value;
            }
        }

        public Dictionary<string, List<string>> SearchResult
        {
            get { return _searchResult; }
            internal set
            {
                _searchResult = value;
                _webParserInfoState.SearchResult = value;
            }
        }
        
        public Exception LastExepction
        {
            get { return _lastException; }
            internal set
            {
                _lastException = value;
                _webParserInfoState.Exception = value;
            }
        }

        public WebParserInfoState WebParserInfoState
        {
            get { return _webParserInfoState; }
        }

        public bool CancelThread
        {
            set { _cancelThread = value; }
            get { return _cancelThread; }
        }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Standard constructor
        /// </summary>
        public WebParser()
        {
            _threadWebParser = new Thread(ThreadFunction);
            _threadWebParser.IsBackground = true;
            _threadWebParser.Name = @"WebParser";
            _threadWebParser.Start();

            EncodingType = Encoding.Default.ToString();
            WebSite = @"";
            State = WebParserState.Idle;
            RegexList = null;
        }

        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="webSiteUrl">URL of the website which should be parsed</param>
        /// <param name="regexList">Dictionary with the regex strings and the regex options for it</param>
        /// <param name="encoding">Encoding for the download content</param>
        public WebParser(string webSiteUrl, RegExList regexList, string encoding) : base ()
        {
            _encodingType = encoding;
            _regexList = regexList;

            // User property for validation
            WebSite = webSiteUrl;
        }

        /// <summary>
        /// Function for the parsing thread
        /// First it checks if a parse process could be started
        /// Then it starts the process
        /// - Loading the page source code
        /// - Parsing the page source code
        /// - Signal of process finish
        /// </summary>
        private void ThreadFunction ()
        {
            while (true)
            {
                if (ThreadRunning)
                {
                    try
                    {
                        lock (thisLockThread)
                        {
                            // Set state
                            State = WebParserState.Started;
                            LastErrorCode = WebParserErrorCodes.Started;
                            LastExepction = null;
                            LastRegexListKey = null;
                            Percent = 0;
                            SetAndSendState(WebParserInfoState);

                            // Check if thread should be canceled
                            if (CancelThread)
                            {
                                LastErrorCode = WebParserErrorCodes.CancelThread;
                                LastExepction = null;
                                Percent = 0;
                                SetAndSendState(WebParserInfoState);
                            }

                            if (ThreadRunning)
                            {
                                // Reset search result
                                if (SearchResult != null && SearchResult.Count > 0)
                                    SearchResult.Clear();

                                // Check if thread should be canceled
                                if (CancelThread)
                                {

                                    LastErrorCode = WebParserErrorCodes.CancelThread;
                                    LastExepction = null;
                                    Percent = 0;
                                    SetAndSendState(WebParserInfoState);
                                }

                                if (ThreadRunning)
                                {
                                    // Set state to loading
                                    State = WebParserState.Loading;
                                    LastErrorCode = WebParserErrorCodes.ContentLoadStarted;
                                    LastExepction = null;
                                    Percent = 5;
                                    SetAndSendState(WebParserInfoState);

                                    // Create web client with the given or default user agent identifier.
                                    using (var client = new WebClient())
                                    {
                                        // Browser identifier (e.g. FireFox 36)
                                        client.Headers["User-Agent"] = UserAgentIdentifier;
                                        // Download content as raw data
#if _DEBUG_THREADFUNCTION
                                        Console.WriteLine(@"WebSide: {0}", _webSite);
#endif
                                        DownloadComplete = false;
                                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                        client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataWebSiteCompleted);
                                        client.DownloadDataAsync(new Uri(_webSite));
                                        while (!DownloadComplete)
                                        {
                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {

                                                LastErrorCode = WebParserErrorCodes.CancelThread;
                                                LastExepction = null;
                                                Percent = 0;
                                                SetAndSendState(WebParserInfoState);
                                                DownloadComplete = false;
                                                client.CancelAsync();
                                                break;
                                            }
                                            Thread.Sleep(10);
                                        }
                                        client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                        client.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(client_DownloadDataWebSiteCompleted);
                                    }

                                    // Check if the website content load was successful and call event
                                    if (WebSiteContent == @"")
                                    {
                                        LastErrorCode = WebParserErrorCodes.NoWebContentLoaded;
                                        LastExepction = null;
                                        Percent = 0;
                                        SetAndSendState(WebParserInfoState);
                                    }
                                    else
                                    {
                                        LastErrorCode = WebParserErrorCodes.ContentLoadFinished;
                                        LastExepction = null;
                                        Percent = 10;
                                        SetAndSendState(WebParserInfoState);
                                    }

                                    // Check if thread should be canceled
                                    if (CancelThread)
                                    {
                                        LastErrorCode = WebParserErrorCodes.CancelThread;
                                        LastExepction = null;
                                        Percent = 0;
                                        SetAndSendState(WebParserInfoState);
                                    }

                                    if (ThreadRunning)
                                    {
                                        // Set state to parsing
                                        State = WebParserState.Parsing;
                                        LastErrorCode = WebParserErrorCodes.SearchStarted;
                                        LastExepction = null;
                                        Percent = 15;
                                        SetAndSendState(WebParserInfoState);

                                        int statusValueStep = (100 - 15) / RegexList.RegexListDictionary.Count;
                                        int statusValue = 15;
#if _DEBUG_THREADFUNCTION
                                        Console.WriteLine("Parsing-Step: {0}", statusValueStep);
#endif

                                        // Loop through the dictionary and fill the result in the result list
                                        foreach (var regexExpression in RegexList.RegexListDictionary)
                                        {
                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {
                                                LastErrorCode = WebParserErrorCodes.CancelThread;
                                                LastExepction = null;
                                                Percent = 0;
                                                SetAndSendState(WebParserInfoState);
                                                break;
                                            }

                                            // Set last regex key
                                            LastRegexListKey = regexExpression.Key;

#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine("Key: {0}", regexExpression.Key);
#endif
                                            var regexElement = regexExpression.Value;

#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine("RegexString: {0}", regexElement.RegexExpresion);
#endif
                                            // Build the reges options
                                            List<RegexOptions> tmpRegexOptionsList = regexElement.RegexOptions;
                                            RegexOptions tmpRegexOptions = RegexOptions.None;

                                            if (tmpRegexOptionsList != null && tmpRegexOptionsList.Count > 0)
                                            {
                                                foreach (var regexOption in tmpRegexOptionsList)
                                                {
                                                    tmpRegexOptions |= regexOption;
                                                }
                                            }

#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine("RegexOptionSet: {0}", tmpRegexOptions);
#endif

                                            // Search for the value
                                            var added = false;
                                            MatchCollection matchCollection = Regex.Matches(WebSiteContent, regexExpression.Value.RegexExpresion, tmpRegexOptions);

                                            // Add the parsing result if a result has been found
                                            if (regexExpression.Value.RegexFoundPosition < matchCollection.Count)
                                            {
                                                if (SearchResult == null)
                                                {
                                                    SearchResult = new Dictionary<string, List<string>>();
                                                }
                                                List<string> listResults = new List<string>();

                                                // If a specific search result should be taken or all results (RegexFoundPosition == -1)
                                                if (regexExpression.Value.RegexFoundPosition >= 0)
                                                {
#if _DEBUG_THREADFUNCTION
                                                    Console.WriteLine(String.Format(@"Value: '{0}' = '{1}'", regexExpression.Key, matchCollection[regexExpression.Value.RegexFoundPosition].Groups[1].Value));
#endif
                                                    if (regexExpression.Value.DownloadResult)
                                                    {

                                                        // Create web client with the given or default user agent identifier.
                                                        using (var client = new WebClient())
                                                        {
                                                            // Browser identifier (e.g. FireFox 36)
                                                            client.Headers["User-Agent"] = UserAgentIdentifier;

                                                            // Download a string
#if _DEBUG_THREADFUNCTION
                                                            Console.WriteLine(@"DownLoad-WebSide: {0}", matchCollection[regexExpression.Value.RegexFoundPosition].Groups[1].Value);
#endif
                                                            DownloadComplete = false;
                                                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                                            client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataContentCompleted);
                                                            client.DownloadDataAsync(new Uri(matchCollection[regexExpression.Value.RegexFoundPosition].Groups[1].Value));
                                                            while (!DownloadComplete)
                                                            {
                                                                // Check if thread should be canceled
                                                                if (CancelThread)
                                                                {

                                                                    LastErrorCode = WebParserErrorCodes.CancelThread;
                                                                    LastExepction = null;
                                                                    Percent = 0;
                                                                    SetAndSendState(WebParserInfoState);
                                                                    DownloadComplete = false;
                                                                    client.CancelAsync();
                                                                }
                                                                Thread.Sleep(10);
                                                            }
                                                            client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                                            client.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(client_DownloadDataContentCompleted);

                                                            listResults.Add(Convert.ToBase64String(DataContent));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 1; i < matchCollection[regexExpression.Value.RegexFoundPosition].Groups.Count; i++)
                                                        {
                                                            // Check if thread should be canceled
                                                            if (CancelThread)
                                                            {
                                                                LastErrorCode = WebParserErrorCodes.CancelThread;
                                                                LastExepction = null;
                                                                Percent = 0;
                                                                SetAndSendState(WebParserInfoState);
                                                            }

                                                            if (matchCollection[regexExpression.Value.RegexFoundPosition].Groups[i].Value != "")
                                                            {
                                                                listResults.Add(matchCollection[regexExpression.Value.RegexFoundPosition].Groups[i].Value);
                                                                i = matchCollection[regexExpression.Value.RegexFoundPosition].Groups.Count;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (Match match in matchCollection)
                                                    {
                                                        // Check if thread should be canceled
                                                        if (CancelThread)
                                                        {
                                                            LastErrorCode = WebParserErrorCodes.CancelThread;
                                                            LastExepction = null;
                                                            Percent = 0;
                                                            SetAndSendState(WebParserInfoState);
                                                        }
#if _DEBUG_THREADFUNCTION
                                                        Console.WriteLine(String.Format(@"Value: '{0}' = '{1}'", regexExpression.Key, match.Groups[1].Value));
#endif
                                                        if (regexExpression.Value.DownloadResult)
                                                        {
                                                            for (int i = 1; i < match.Groups.Count; i++)
                                                            {
                                                                // Check if thread should be canceled
                                                                if (CancelThread)
                                                                {
                                                                    LastErrorCode = WebParserErrorCodes.CancelThread;
                                                                    LastExepction = null;
                                                                    Percent = 0;
                                                                    SetAndSendState(WebParserInfoState);
                                                                }

                                                                if (match.Groups[i].Value != "")
                                                                {

                                                                    // Create web client with the given or default user agent identifier.
                                                                    using (var client = new WebClient())
                                                                    {
                                                                        // Browser identifier (e.g. FireFox 36)
                                                                        client.Headers["User-Agent"] = UserAgentIdentifier;

                                                                        // Download a string
#if _DEBUG_THREADFUNCTION
                                                                        Console.WriteLine(@"DownLoad-WebSide: {0}", match.Groups[i].Value);
#endif
                                                                        DownloadComplete = false;
                                                                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                                                        client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataContentCompleted);
                                                                        client.DownloadDataAsync(new Uri(match.Groups[i].Value));
                                                                        while (!DownloadComplete)
                                                                        {
                                                                            // Check if thread should be canceled
                                                                            if (CancelThread)
                                                                            {

                                                                                LastErrorCode = WebParserErrorCodes.CancelThread;
                                                                                LastExepction = null;
                                                                                Percent = 0;
                                                                                SetAndSendState(WebParserInfoState);
                                                                                DownloadComplete = false;
                                                                                client.CancelAsync();
                                                                            }
                                                                            Thread.Sleep(10);
                                                                        }
                                                                        client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                                                        client.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(client_DownloadDataContentCompleted);
                                                                        listResults.Add(Convert.ToBase64String(DataContent));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = 1; i < match.Groups.Count; i++)
                                                            {
                                                                // Check if thread should be canceled
                                                                if (CancelThread)
                                                                {
                                                                    LastErrorCode = WebParserErrorCodes.CancelThread;
                                                                    LastExepction = null;
                                                                    Percent = 0;
                                                                    SetAndSendState(WebParserInfoState);
                                                                }

                                                                if (match.Groups[i].Value != "")
                                                                {
                                                                    listResults.Add(match.Groups[i].Value);
//                                                                    i = match.Groups.Count;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                SearchResult.Add(regexExpression.Key, listResults);
                                                added = true;
                                            }

                                            // Check if no result has been found or is not added and the result can not be empty
                                            if ((matchCollection.Count == 0 || added == false) && !regexElement.ResultEmpty)
                                            {
#if _DEBUG_THREADFUNCTION
                                                Console.WriteLine(String.Format(@"No MATCH found!"));
#endif
                                                LastErrorCode = WebParserErrorCodes.ParsingFailed;
                                                LastExepction = null;
                                                Percent = 0;
                                                SetAndSendState(WebParserInfoState);
                                                break;
                                            }

                                            statusValue += statusValueStep;

                                            if (statusValue < 100)
                                            {
                                                LastErrorCode = WebParserErrorCodes.SearchRunning;
                                                LastExepction = null;
                                                Percent = statusValue;
                                                SetAndSendState(WebParserInfoState);
                                            }
                                        }

                                        if (ThreadRunning)
                                        {
                                            LastErrorCode = WebParserErrorCodes.SearchFinished;
                                            LastExepction = null;
                                            Percent = 100;
                                            SetAndSendState(WebParserInfoState);
                                        }

                                        // Check if thread should be canceled
                                        if (CancelThread)
                                        {
                                            LastErrorCode = WebParserErrorCodes.CancelThread;
                                            LastExepction = null;
                                            Percent = 0;
                                            SetAndSendState(WebParserInfoState);
                                        }

                                        if (ThreadRunning)
                                        {
                                            // Signal that the thread has finished
                                            LastErrorCode = WebParserErrorCodes.Finished;
                                            LastExepction = null;
                                            Percent = 100;
                                            SetAndSendState(WebParserInfoState);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (WebException webEx)
                    {
                        // Set state
                        State = WebParserState.Idle;
                        LastErrorCode = WebParserErrorCodes.WebExceptionOccured;
                        LastExepction = webEx;
                        Percent = 0;
                        SetAndSendState(WebParserInfoState);
                    }
                    catch (Exception ex)
                    {
                        // Set state
                        State = WebParserState.Idle;
                        LastErrorCode = WebParserErrorCodes.ExceptionOccured;
                        LastExepction = ex;
                        Percent = 0;
                        SetAndSendState(WebParserInfoState);
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            WebParserInfoState.PercentageDownload = e.ProgressPercentage;
        }

        /// <summary>
        /// This function sets the downloaded website content to the class variable
        /// </summary>
        /// <param name="sender">Webclient which download the website content asynchron</param>
        /// <param name="e">DownloadDataCompletedEventArgs with the result</param>
        public void client_DownloadDataWebSiteCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    if (e.Result.LongLength > 0)
                        WebSiteContent = Encoding.UTF8.GetString(e.Result);
                    else
                        WebSiteContent = "";

                    WebParserInfoState.PercentageDownload = 100;
                    DownloadComplete = true;
                }
            }
            catch (WebException webEx)
            {
                // Set state
                State = WebParserState.Idle;
                LastErrorCode = WebParserErrorCodes.WebExceptionOccured;
                LastExepction = webEx;
                Percent = 0;
                SetAndSendState(WebParserInfoState);
            }
        }

        /// <summary>
        /// This function sets the downloaded website content to the class variable
        /// </summary>
        /// <param name="sender">Webclient which download the website data content asynchron</param>
        /// <param name="e">DownloadDataCompletedEventArgs with the result</param>
        public void client_DownloadDataContentCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    if (e.Result.LongLength > 0)
                        DataContent = e.Result;
                    else
                        Array.Clear(DataContent, 0, DataContent.Length);

                    DownloadComplete = true;
                }
            }
            catch (WebException webEx)
            {
                // Set state
                State = WebParserState.Idle;
                LastErrorCode = WebParserErrorCodes.WebExceptionOccured;
                LastExepction = webEx;
                Percent = 0;
                SetAndSendState(WebParserInfoState);
            }
        }

        /// <summary>
        /// This function starts the parsring process.
        /// </summary>
        /// <returns>Start process return code</returns>
        public bool StartParsing()
        {
            try
            {
                lock (thisLockStarting)
                {
                    // Send start event to the GUI
                    LastErrorCode = WebParserErrorCodes.Starting;
                    LastExepction = null;
                    Percent = 0;
                    SetAndSendState(WebParserInfoState);

                    // Check if a new parsing could be started
                    if (State != WebParserState.Idle)
                    {
                        LastErrorCode = WebParserErrorCodes.BusyFailed;
                        LastExepction = null;
                        Percent = 0;
                        SetAndSendState(WebParserInfoState);
                        return false;
                    }

                    if (WebSite == @"invalid")
                    {
                        LastErrorCode = WebParserErrorCodes.InvalidWebSiteGiven;
                        LastExepction = null;
                        Percent = 0;
                        SetAndSendState(WebParserInfoState);
                        return false;
                    }

                    if (RegexList == null || RegexList.RegexListDictionary.Count == 0)
                    {
                        LastErrorCode = WebParserErrorCodes.NoRegexListGiven;
                        LastExepction = null;
                        Percent = 0;
                        SetAndSendState(WebParserInfoState);
                        return false;
                    }
                }

                ThreadRunning = true;

                return true;
            }
            catch (Exception ex)
            {
                // Set state
                State = WebParserState.Idle;
                LastErrorCode = WebParserErrorCodes.ExceptionOccured;
                LastExepction = ex;
                Percent = 0;
                SetAndSendState(WebParserInfoState);
                return false;
            }
        }

        /// <summary>
        /// This function sets the current info state and sents
        /// the state to the GUI
        /// </summary>
        /// <param name="webParserInfoState">WebParserInfoState</param>
        void SetAndSendState(WebParserInfoState webParserInfoState)
        {
#if DEBUG
            Console.WriteLine(@"State: {0} / ThreadRunning: {1} / ErrorCode: {2} / Percent: {3}", State, ThreadRunning, webParserInfoState.LastErrorCode, webParserInfoState.Percentage);
#endif
            if (OnWebParserUpdate != null)
            {
                if (ThreadRunning)
                    OnWebParserUpdate(this, new OnWebParserUpdateEventArgs(webParserInfoState));
            }

            // Set state to "idle"
            if (webParserInfoState.LastErrorCode == WebParserErrorCodes.Finished || webParserInfoState.LastErrorCode < 0)
            {
                ThreadRunning = false;
                State = WebParserState.Idle;
                CancelThread = false;
            }
        }

        #endregion Methodes

        #region Events / Delegates

        public delegate void WebParserUpdateEventHandler(object sender, OnWebParserUpdateEventArgs e);

        public event WebParserUpdateEventHandler OnWebParserUpdate;

        #endregion Events / Delegates

    }

    /// <summary>
    /// Enum for the WebParser state
    /// </summary>
    public enum WebParserState
    {
        Idle,
        Started,
        Loading,
        Parsing
    };

    /// <summary>
    /// ErrorCodes
    /// </summary>
    public enum WebParserErrorCodes
    {
        Finished = 8,
        SearchFinished = 7,
        SearchRunning = 6,
        SearchStarted = 5,
        ContentLoadFinished = 4,
        ContentLoadStarted = 3,
        Started = 2,
        Starting = 1,
        NoError = 0,
        StartFailed = -1,
        BusyFailed = -2,
        InvalidWebSiteGiven = -3,
        NoRegexListGiven = -4,
        NoWebContentLoaded = -5,
        ParsingFailed = -6,
        CancelThread = - 7,
        WebExceptionOccured = -8,
        ExceptionOccured = -9
    }

    /// <summary>
    /// Class of the current info state of the WebParser
    /// </summary>
    public class WebParserInfoState
    {
        #region Variables

        /// <summary>
        /// Current url of the WebParser
        /// </summary>
        private string _webSite;

        /// <summary>
        /// Current user agent identifier of the WebParser
        /// </summary>
        private string _userAgentIdentifier;

        /// <summary>
        /// Current state of the WebParser
        /// </summary>
        private WebParserState _state;

        /// <summary>
        /// Percentage of the update process
        /// </summary>
        private int _percentageUpdate;
        
        /// <summary>
        /// Perentage of the download process of the website or data content
        /// </summary>
        private int _percentageDownload;

        /// <summary>
        /// Last error code of the WebParser
        /// </summary>
        private WebParserErrorCodes _lastErrorCode;

        /// <summary>
        /// Current regular expression list of the WebParser
        /// </summary>
        private RegExList _regexList;

        /// <summary>
        /// Last _regexList key
        /// </summary>
        private string _lastRegexKey;

        /// <summary>
        /// Dictionary with the current search result of the WebParser
        /// Key is the regex string
        /// Value is the search result of the regex
        /// </summary>
        private Dictionary<string, List<string>> _searchResult;

        /// <summary>
        /// Exception if an exception occurred
        /// </summary>
        private Exception _lastException;

        #endregion Variables

        #region Properties

        public string WebSite
        {
            get { return _webSite; }
            internal set { _webSite = value; }
        }

        public string UserAgentIdentifier
        {
            get { return _userAgentIdentifier; }
            internal set { _userAgentIdentifier = value; }
        }

        public WebParserState State
        {
            get { return _state; }
            internal set { _state = value; }
        }

        public int Percentage
        {
            get { return _percentageUpdate; }
            internal set { _percentageUpdate = value; }
        }

        public int PercentageDownload
        {
            get { return _percentageDownload; }
            internal set { _percentageDownload = value; }
        }

        public WebParserErrorCodes LastErrorCode
        {
            get { return _lastErrorCode; }
            internal set { _lastErrorCode = value; }
        }

        public RegExList RegexList
        {
            get { return _regexList; }
            internal set { _regexList = value; }
        }

        public string LastRegexListKey
        {
            get { return _lastRegexKey; }
            internal set { _lastRegexKey = value; }
        }

        public Dictionary<string, List<string>> SearchResult
        {
            get { return _searchResult; }
            internal set { _searchResult = value; }
        }

        public Exception Exception
        {
            get { return _lastException; }
            internal set { _lastException = value; }
        }

        #endregion Properties
    }

    /// <summary>
    /// Class of the eventargs of the event OnWebSiteLoadFinished
    /// </summary>
    public class OnWebParserUpdateEventArgs : EventArgs
    {
        #region Variables

        /// <summary>
        /// State of the website load
        /// </summary>
        private WebParserInfoState _webParserInfoState;

        #endregion Variables

        #region Properties

        public WebParserInfoState WebParserInfoState
        {
            get { return _webParserInfoState; }
        }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webParserInfoState">Last error code of the webparser</param>
        /// Error code see the class "WebParser"
        /// <param name="percent">Value in percent of the process run</param>
        /// <param name="exception">Exception which maybe occurred</param>
        public OnWebParserUpdateEventArgs (WebParserInfoState webParserInfoState)
	    {
		    _webParserInfoState = webParserInfoState;
        }

        #endregion Methodes
    }
}
