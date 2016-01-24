using System;
using System.Windows.Forms;
using WebParser;

namespace WebParserTester
{
    public partial class frmWebParserTester : Form
    {
        #region Variables

        /// <summary>
        /// Instance of the ThreadFunctions
        /// </summary>
        private ThreadFunctions _threadFunctions;

        public ThreadFunctions.UpdateGuiEventHandler OnUpdateGuiEvent { get; private set; }

        /// <summary>
        /// Global flag for the test result
        /// This flag must be set if any testcase failed
        /// </summary>
        private bool _resultFlag;

        #endregion Variables

        #region Properties
        #endregion Properties

        #region Methodes

        public frmWebParserTester()
        {
            InitializeComponent();

            _threadFunctions = new ThreadFunctions(this);

            _threadFunctions.OnUpdateGuiEvent -= OnUpdateGui;
            _threadFunctions.OnUpdateGuiEvent += OnUpdateGui;
            _resultFlag = true;
        }

        private void frmWebParserTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;
            _threadFunctions.OnUpdateGuiEvent -= OnUpdateGui;
            _threadFunctions.ThreadStop = true;
        }

        /// <summary>
        /// This function starts the test process
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">EventArgs</param>
        private void btnStartTest_Click(object sender, EventArgs e)
        {
            // Reset variables
            _resultFlag = true;
            richTextBoxResult.Clear();

            btnStartTest.Enabled = false;
            btnStopTest.Enabled = true;

            _threadFunctions.StartTestCaseThread();
        }

        /// <summary>
        /// This function stops the test process
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">EventArgs</param>
        private void btnStopTest_Click(object sender, EventArgs e)
        {
            _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

            // Set cancel flag
            _threadFunctions.CancelFlag = true;

            btnStopTest.Enabled = false;
        }

        #endregion Methodes

        #region Events / Delegates

        /// <summary>
        /// This function updates the test report
        /// </summary>
        /// <param name="sender">Webparser</param>
        /// <param name="e">OnWebSiteLoadFinishedEventArgs</param>
        public void OnUpdate (object sender, OnWebParserUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUpdate(sender, e)));
            }
            else
            {
                // Write state to the richedit box
                HelperFunctions.AddTestCaseStateToReport(richTextBoxResult, e);

                Console.WriteLine("CaseCounter: " + _threadFunctions.TestCaseCounter + " / LastErrorCode: " + e.WebParserInfoState.LastErrorCode + " / GUI: " + _threadFunctions.GuiUpdateCounter);
                switch (_threadFunctions.TestCaseCounter)
                {
                    case 1:
                        {
                            int guiUpdateCounterReached = 2;
                            if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.Started)
                            {
                                _threadFunctions.WebParser.StartParsing();
                            }

                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;
                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.BusyFailed)
                                {
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                }
                                else
                                {
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelWebParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 2:
                        {
                            int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

                                Console.WriteLine(" LastErrorCode: " + e.WebParserInfoState.LastErrorCode + " / GUI: " + _threadFunctions.GuiUpdateCounter);
                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.InvalidWebSiteGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.WebParser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelWebParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 3:
                        {
                            int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.NoRegexListGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.WebParser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelWebParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 4:
                        {
                            int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.InvalidWebSiteGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.WebParser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelWebParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 5:
                        {
                            int guiUpdateCounterReached = 5;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.ParsingFailed)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.WebParser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                    CancelWebParserStartNextTestCase();
                            }
                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 6:
                        {
                            int guiUpdateCounterReached = 6;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

                                if (e.WebParserInfoState.LastErrorCode == WebParserErrorCodes.Finished 
                                    && _threadFunctions.WebParser.SearchResult.Count == 1
                                    && e.WebParserInfoState.SearchResult["Gesamt"] == "Anlage")
                                        HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.WebParser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.WebParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                    CancelWebParserStartNextTestCase();
                            }
                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// This function updates the test report
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">GuiUpdateEventArgs</param>
        public void OnUpdateGui(object sender, ThreadFunctions.GuiUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUpdateGui(sender, e)));
            }
            else
            {
                switch (e.State)
                {
                    case ThreadFunctions.GuiUpdateState.ProcessStartSucess:
                        {
                            HelperFunctions.AddTestProcessStartToReport(richTextBoxResult);
                            break;
                        }
                    case ThreadFunctions.GuiUpdateState.ProcessCancel:
                        {
                            btnStartTest.Enabled = true;
                            btnStopTest.Enabled = false;
                            HelperFunctions.AddTestCancelToReport(richTextBoxResult);
                            break;
                        }
                    case ThreadFunctions.GuiUpdateState.TestCaseStart:
                        {
                            HelperFunctions.AddTestCaseStartToReport(richTextBoxResult, e.Messages[0]);
                            break;
                        }
                    case ThreadFunctions.GuiUpdateState.TestCaseResult:
                        {
                            btnStartTest.Enabled = true;
                            btnStopTest.Enabled = false;
                            // This check must always the last one!!!
                            // If a new test case will be added, add it before of this case
                            HelperFunctions.AddTestProcessFinishResultToReport(richTextBoxResult, _resultFlag);
                            break;
                        }
                    case ThreadFunctions.GuiUpdateState.ProcessFinish:
                        {
                            btnStartTest.Enabled = true;
                            btnStopTest.Enabled = false;
                            // This check must always the last one!!!
                            // If a new test case will be added, add it before of this case
                            HelperFunctions.AddTestProcessFinishResultToReport(richTextBoxResult, _resultFlag);
                            break;
                        }
                }
            }
        }

        #endregion Events / Delegates

        #region WebParser error occoured

        /// <summary>
        /// This function cancels the WebParser and starts the next test case
        /// </summary>
        private void CancelWebParserStartNextTestCase()
        {
            _threadFunctions.WebParser.OnWebParserUpdate -= OnUpdate;

            _threadFunctions.WebParser.CancelThread = true;
            HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
            _resultFlag = false;
            _threadFunctions.NextTestCaseFlag = true;
        }

        #endregion WebParser error occoured
    }
}
