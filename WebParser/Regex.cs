using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebParser
{
    /// <summary>
    /// Class of a regex list
    /// This class store the name and a RegexElement
    /// in a dictionary.
    /// The name is used for creating the result dictionary.
    /// </summary>
    public class RegExList
    {
        #region Variables

        /// <summary>
        /// A dictionary with a name for the regex search string
        /// and a RegexElement.
        /// For more details about the "RegExElement" take a look
        /// at the class of the type.
        /// </summary>
        private Dictionary<string, RegexElement> _regexList;

        /// <summary>
        /// Last thrown exception of the class
        /// </summary>
        private Exception _lastException;

        #endregion Variables

        #region Properties

        public Dictionary<string, RegexElement> RegexListDictionary
        {
            set { _regexList = value; }
            get { return _regexList; }
        }

        public Exception LastException
        {
            get { return _lastException; }
        }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Constructor for building a RegExList instance
        /// </summary>
        public RegExList()
        {
            try
            {
                _regexList = new Dictionary<string, RegexElement>();
                _lastException = null;
            }
            catch (Exception ex)
            {
                _regexList = null;
                _lastException = ex;
            }
        }


        /// <summary>
        /// Constructor for building a RegExList instance
        /// </summary>
        /// <param name="name">Name of the regex. This name will be used for creating the result dictionary</param>
        /// <param name="RegexElement">RegexElement with the regex search string and with the optional regex options</param>
        public RegExList(string name, RegexElement regexElement)
        {
            try
            {
                _regexList = new Dictionary<string, RegexElement>();
                _regexList.Add(name, regexElement);
                _lastException = null;
            }
            catch (Exception ex)
            {
                _regexList = null;
                _lastException = ex;
            }
        }

        /// <summary>
        /// This function adds a new entry to the dictionary
        /// The return value indicates if the add was successful.
        /// If the add failed the value "LastException" stores the exception which had been occurred.
        /// </summary>
        /// <param name="name">Name of the regex expression. This name will be used for creating the result dictionary</param>
        /// <param name="RegexElement">RegexElement with the regex search string and with the optional regex options</param>
        /// <returns>Flag if the add was successful </returns>
        /// true  = successfull
        /// false = failed
        public bool Add(string name, RegexElement regexElement)
        {
            try
            {
                _regexList.Add(name, regexElement);

                return true;
            }
            catch (Exception ex)
            {
                _lastException = ex;
                return false;
            }
        }

        #endregion Methodes
    }

    /// <summary>
    /// Class of a RegexElement
    /// This class stores the regex search string and
    /// if necessary a list of regex options
    /// </summary>
    public class RegexElement
    {
        #region Variables

        /// <summary>
        /// String with the regex expression
        /// </summary>
        private string _regexExpression;

        /// <summary>
        /// List with the regex option for the search
        /// </summary>
        private List<RegexOptions> _regexOptions;

        /// <summary>
        /// Index of the found position
        /// </summary>
        private int _regexFoundPosition;

        /// <summary>
        /// Flag if a parsing result can be empty
        /// </summary>
        private bool _resultEmpty;

        #endregion Variables

        #region Properties

        public string RegexExpresion
        {
            set { _regexExpression = value; }
            get { return _regexExpression; }
        }

        public List<RegexOptions> RegexOptions
        {
            set { _regexOptions = value; }
            get { return _regexOptions; }
        }

        public bool ResultEmpty
        {
            set { _resultEmpty = value; }
            get { return _resultEmpty; }
        }

        public int RegexFoundPosition
        {
            set { _regexFoundPosition = value; }
            get { return _regexFoundPosition; }
        }

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Constructor for building a RegexElement instance
        /// </summary>
        /// <param name="regexExpression">The string for the search string of the regex</param>
        /// <param name="regexFoundPostion">The index of the found value</param>
        /// <param name="resultEmpty">Flag if the parsing result can be empty</param>
        /// <param name="regexOptions">The list with the regex options. This parameter is optional</param>
        public RegexElement(string regexExpression, int regexFoundPostion, bool resultEmpty, List<RegexOptions> regexOptions = null)
        {
            _regexExpression = regexExpression;
            _regexFoundPosition = regexFoundPostion;
            _resultEmpty = resultEmpty;
            _regexOptions = regexOptions;
        }

        #endregion Methodes
    }
}
