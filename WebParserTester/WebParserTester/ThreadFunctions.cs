using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using WebParser;

namespace WebParserTester
{
    public class ThreadFunctions
    {
        #region Variables

        frmWebParserTester _frmWebParserTester;

        /// <summary>
        /// Global flag for starting the thread
        /// </summary>
        private bool _threadStartFlag = false;

        /// <summary>
        /// Global flag for stopping the thread
        /// </summary>
        private bool _threadStopFlag = false;

        /// <summary>
        /// Counter for the test case
        /// </summary>
        private int _testCaseCounter = 0;

        /// <summary>
        /// Counter for the GUI updates
        /// </summary>
        private int _guiUpdateCounter = 0;

        /// <summary>
        /// Global flag for starting the next testcase
        /// </summary>
        private bool _nextTestCaseFlag = true;

        /// <summary>
        /// Global flag for stopping the test process
        /// </summary>
        private bool _cancelFlag = false;

        /// <summary>
        /// Global flag for signal that the test process has been finished
        /// </summary>
        private bool _finishFlag = false;

        /// <summary>
        /// Thread for the testcase automation
        /// </summary>
        private Thread _threadTestCase;

        /// <summary>
        /// Webparser for the thread
        /// </summary>
        private WebParser.WebParser _webParser;

        /// <summary>
        /// States for the GUI update of the test report
        /// </summary>
        public enum GuiUpdateState
        {
            ProcessStartSucess = 1,
            ProcessStartFailed = 2,
            TestCaseStart = 3,
            TestCaseResult = 4,
            ProcessFinish = 5,
            ProcessCancel = 6
        };

        #endregion Variables

        #region Properties

        public bool ThreadStop
        {
            set { _threadStopFlag = value; }
            get { return _threadStopFlag; }
        }

        public bool ThreadStart
        {
            set { _threadStartFlag = value; }
            get { return _threadStartFlag; }
        }

        public int TestCaseCounter
        {
            internal set { _testCaseCounter = value; }
            get { return _testCaseCounter; }
        }

        public int GuiUpdateCounter
        {
            internal set { _guiUpdateCounter = value; }
            get { return _guiUpdateCounter; }
        }

        public bool NextTestCaseFlag
        {
            internal set { _nextTestCaseFlag = value; }
            get { return _nextTestCaseFlag; }
        }

        public bool CancelFlag
        {
            set { _cancelFlag = value; }
            get { return _cancelFlag; }
        }

        public bool FinishFlag
        {
            internal set { _finishFlag = value; }
            get { return _finishFlag; }
        }

        public WebParser.WebParser WebParser
        {
            internal set { _webParser = value; }
            get { return _webParser; }
        }

        #endregion Properties

        #region Delegates / Events

        public delegate void UpdateGuiEventHandler(object sender, GuiUpdateEventArgs e);

        public event UpdateGuiEventHandler OnUpdateGuiEvent;

        #endregion Delegates / Events

        public ThreadFunctions(frmWebParserTester frmWebParserTester)
        {
            _frmWebParserTester = frmWebParserTester;

            WebParser = new WebParser.WebParser();

            _threadTestCase = new Thread(ThreadTestCases);
            _threadTestCase.Name = @"Testautomation";
            _threadTestCase.Start();
        }

        /// <summary>
        /// This function starts the test automation
        /// </summary>
        public void StartTestCaseThread()
        {
            _threadStartFlag = true;
        }

        /// <summary>
        /// This function does the test automation
        /// </summary>
        private void ThreadTestCases()
        {
            while (true)
            {
                if (_threadStopFlag)
                    break;

                while (_threadStartFlag && !_cancelFlag && !_finishFlag)
                {
                    if (_testCaseCounter == 0)
                    {
                        if (OnUpdateGuiEvent != null)
                            OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartSucess));
                    }

                    if (_nextTestCaseFlag)
                    {
                        Thread.Sleep(100);
                        _nextTestCaseFlag = false;

                        while (_webParser.State != WebParserState.Idle)
                        {
                            Thread.Sleep(100);
                        }

                        // Increase the test case counter to start the next test case
                        _testCaseCounter++;

                        // Reset GUI update counter
                        _guiUpdateCounter = 0;

                        if (_guiUpdateCounter == 0)
                        {
                            WebParser.OnWebParserUpdate -= _frmWebParserTester.OnUpdate;
                            WebParser.OnWebParserUpdate += _frmWebParserTester.OnUpdate;
                        }

                        // Add new test cases here
                        switch (_testCaseCounter)
                        {

                            case 1:
                                {
                                    CheckWebParserStillWorking();
                                    break;
                                }
                            case 2:
                                {
                                    CheckWebParserUrlGiven();
                                    break;
                                }
                            case 3:
                                {
                                    CheckWebParserRegexListGiven();
                                    break;
                                }
                            case 4:
                                {
                                    CheckWebParserInvalidUrlGiven();
                                    break;
                                }
                            case 5:
                                {
                                    CheckWebParserError();
                                    break;
                                }
                            case 6:
                                {
                                    CheckWebParserSuccessful();
                                    break;
                                }
                            case 7:
                                {
                                    _finishFlag = true;
                                    break;
                                }
                        }
                    }
                }

                // Check if the test process should be canceled
                if (_cancelFlag)
                {
                    if (OnUpdateGuiEvent != null)
                        OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessCancel));

                    _cancelFlag = false;
                    _nextTestCaseFlag = true;
                    _threadStartFlag = false;
                    _testCaseCounter = 0;
                }

                if (_finishFlag)
                {
                    if (OnUpdateGuiEvent != null)
                        OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessFinish));

                    _finishFlag = false;
                    _nextTestCaseFlag = true;
                    _threadStartFlag = false;
                    _testCaseCounter = 0;
                }
            }
        }

        #region Testcase functions

        /// <summary>
        /// This testcase checks if the webparser is still working
        /// </summary>
        private void CheckWebParserStillWorking()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Create RegexList
            RegExList regexList = new RegExList(@"FirstRegex", new RegexElement(@"RegexString1", 1, true, false, new List<RegexOptions>() { RegexOptions.None }));
            regexList.Add(@"SecondRegex", new RegexElement(@"RegexString2", 1, false, false, new List<RegexOptions>() { RegexOptions.Singleline, RegexOptions.IgnoreCase }));
            // Set regexlist to webparser
            _webParser.RegexList = regexList;
            // Set website to webparser
            _webParser.WebSite = @"http://www.google.com";
            // Start parsing
            _webParser.StartParsing();
        }

        /// <summary>
        /// This testcase checkes if the webparser has a url given
        /// </summary>
        private void CheckWebParserUrlGiven()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to a empty string;
            _webParser.WebSite = @"";

            // Check if the parsing process has been started
            if (!_webParser.StartParsing())
            {
                if (OnUpdateGuiEvent != null)
                    OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }

        }

        /// <summary>
        /// This testcase checkes if the webparser has a regexlist given
        /// </summary>
        private void CheckWebParserRegexListGiven()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the webparser
            _webParser.WebSite = @"http://www.google.com";
            // Set regexlist to null in the webparser
            _webParser.RegexList = null;

            // Check if the parsing process has been started
            if (!_webParser.StartParsing())
            {
                if (OnUpdateGuiEvent != null)
                    OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This testcase checkes when a invalid url is given
        /// </summary>
        private void CheckWebParserInvalidUrlGiven()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the webparser
            _webParser.WebSite = @"http://wwwgoogle";
            // Set regexlist to null in the webparser
            _webParser.RegexList = null;

            // Check if the parsing process has been started
            if (!_webParser.StartParsing())
            {
                if (OnUpdateGuiEvent != null)
                    OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This testcase checkes if the search is successful
        /// </summary>
        private void CheckWebParserError()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the webparser
            _webParser.WebSite = @"http://tbarth.eu/sunnyconnectoranalyzer";
            // Create RegexList
            RegExList regexList = new RegExList(@"Gesamt", new RegexElement(@">Gsamt-(.*?)<", 0, false, false, new List<RegexOptions>() { RegexOptions.None }));
            // Set regexlist to webparser
            _webParser.RegexList = regexList;

            // Check if the parsing process has been started
            if (!_webParser.StartParsing())
            {
                if (OnUpdateGuiEvent != null)
                    OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This testcase checkes if the search is successful
        /// </summary>
        private void CheckWebParserSuccessful()
        {
            // Add start entry to report
            if (OnUpdateGuiEvent != null)
                OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the webparser
            _webParser.WebSite = @"http://tbarth.eu/sunnyconnectoranalyzer";
            //_webParser.WebSite = @"http://www.immobilienscout24.de/expose/86810409?PID=60978745&ftc=9004EXPXXUA&utm_medium=email&utm_source=system&utm_campaign=default_fulfillment&utm_content=default_expose&CCWID=$CWID_CONTACT$";
            // Create RegexList
            RegExList regexList = new RegExList(@"Gesamt", new RegexElement(@">Gesamt-(.*?)<", -1, false, false, new List<RegexOptions>() { RegexOptions.None }));
            //RegExList regexList = new RegExList(@"PIC", new RegexElement("data-ng-non-bindable data-src=\"(.*?)\"", 0, true, true, new List<RegexOptions>() { RegexOptions.None }));
            // Set regexlist to webparser
            _webParser.RegexList = regexList;

            // Check if the parsing process has been started
            if (!_webParser.StartParsing())
            {
                if (OnUpdateGuiEvent != null)
                    OnUpdateGuiEvent(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        #endregion Thread functions

        /// <summary>
        /// Class for the GUI update via testcase process thread
        /// </summary>
        public class GuiUpdateEventArgs : EventArgs
        {
            #region Variables

            /// <summary>
            /// Update state
            /// </summary>
            private GuiUpdateState _state;


            /// <summary>
            /// Update message list
            /// </summary>
            private List<string> _messages;

            #endregion Variables

            #region Properties

            public GuiUpdateState State
            {
                get { return _state; }
            }

            public List<string> Messages
            {
                get { return _messages; }
            }

            #endregion Properties

            #region Methodes

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="state">State of the update</param>
            /// <param name="messages">Messages of the update</param>
            public GuiUpdateEventArgs(GuiUpdateState state, List<string> messages = null)
            {
                _state = state;
                _messages = messages;
            }

            #endregion Methodes
        }
    }
}
