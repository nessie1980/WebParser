using System;
using System.Drawing;
using System.Windows.Forms;
using WebParser;

namespace WebParserTester
{
    static public class HelperFunctions
    {
        /// <summary>
        /// This function add the start test process fail entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        /// <returns>Flag that the test process start failed</returns>
        static public bool AddTestProcessFailedToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText(String.Format("Test case start "));
            richTextBoxResult.SelectionColor = Color.Red;
            richTextBoxResult.AppendText("FAILED");
            richTextBoxResult.AppendText(String.Format("{0}{1}", Environment.NewLine, Environment.NewLine));

            return false;
        }

        /// <summary>
        /// This function add the start test process entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        static public void AddTestProcessStartToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText(String.Format("Test process started{0}{1}", Environment.NewLine, Environment.NewLine));
        }

        /// <summary>
        /// This function add the start entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        /// <param name="testCaseName">Name of the test case</param>
        static public void AddTestCaseStartToReport(RichTextBox richTextBoxResult, string testCaseName)
        {
            richTextBoxResult.AppendText(String.Format("========================================================================{0}", Environment.NewLine));
            richTextBoxResult.AppendText(String.Format("Start test case: \"{0}\"{1}{2}", testCaseName, Environment.NewLine, Environment.NewLine));
        }

        static public void AddTestCaseStateToReport(RichTextBox richTextBoxResult, OnWebParserUpdateEventArgs e)
        {
            // Only past the webparser result if the parsing failed
            if (e.WebParserInfoState.Exception == null)
            {
                if (e.WebParserInfoState.LastErrorCode < 0)
                    richTextBoxResult.AppendText("Error:\t");
                else
                    richTextBoxResult.AppendText("Status:\t");

                richTextBoxResult.AppendText(
                    String.Format("{0} ({1} %){2}",
                    e.WebParserInfoState.LastErrorCode.ToString(), e.WebParserInfoState.Percent.ToString(), Environment.NewLine, Environment.NewLine)
                    );
            }
            else
            {
                if (e.WebParserInfoState.LastErrorCode < 0)
                    richTextBoxResult.AppendText("Error:\t");
                else
                    richTextBoxResult.AppendText("Status:\t");

                richTextBoxResult.AppendText(
                    String.Format("{0} ({1} %){2}{3}{4}",
                    e.WebParserInfoState.LastErrorCode.ToString(), e.WebParserInfoState.Percent.ToString(), Environment.NewLine, e.WebParserInfoState.Exception.Message, Environment.NewLine)
                    );
            }
        }

        /// <summary>
        /// This function add the test case result entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        static public void AddTestCaseResultToReport(RichTextBox richTextBoxResult, bool result)
        {
            richTextBoxResult.AppendText(String.Format("{0}Result: ", Environment.NewLine));

            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
                richTextBoxResult.AppendText("PASSED");
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
                richTextBoxResult.AppendText("FAILED");
            }

            richTextBoxResult.SelectionColor = Color.Black;
            richTextBoxResult.AppendText(String.Format("{0}========================================================================", Environment.NewLine));
            richTextBoxResult.AppendText(String.Format("{0}{1}", Environment.NewLine, Environment.NewLine));
        }

        /// <summary>
        /// This function add the finish test process result entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        /// <param name="result">Test finish result</param>
        static public void AddTestProcessFinishResultToReport(RichTextBox richTextBoxResult, bool result)
        {
            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
            }
            richTextBoxResult.AppendText(String.Format("========================================================================{0}", Environment.NewLine));


            richTextBoxResult.SelectionColor = Color.Black;
            richTextBoxResult.AppendText(String.Format("Test process finished{0}{1}END-Result: ", Environment.NewLine, Environment.NewLine));

            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
                richTextBoxResult.AppendText("PASSED");
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
                richTextBoxResult.AppendText("FAILED");
            }

            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
            }
            richTextBoxResult.AppendText(String.Format("{0}========================================================================{1}", Environment.NewLine, Environment.NewLine));
        }

        /// <summary>
        /// This function add the cancel text process entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Richeditbox for the test result output</param>
        static public void AddTestCancelToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText(String.Format("{0}Test process: ", Environment.NewLine));
            richTextBoxResult.SelectionColor = Color.Red;
            richTextBoxResult.AppendText("CANELLED");
        }
    }
}
