using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;

/// <summary>
/// Eng to Kor 
/// Kor to Eng (new Regex("<span class = "fnt_e[0-9]>.+</span>");
/// </summary>

namespace WindowsFormsApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.TopMost = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetBottomLeft();
        }

        private void getKorWords(string _html)
        {
            //품사들 ^^
            string[] PartOfSpeech = { "명사", "대명사", "형용사", "동사", "부사", "전치사", "접속사", "감탄사" };
            string resultstring = null;

            //한글단어 뜻이 있는 클래스 추출하는 정규식
            Regex ExtKorClasses = new Regex("<span class=\"fnt_k0[0-9]\">.+</span>", RegexOptions.IgnoreCase & RegexOptions.IgnorePatternWhitespace);

            //추출한 클래스중에서 한글만 뽑아내는 정규식
            Regex ExtKor = new Regex("[ㄱ-힗]", RegexOptions.IgnoreCase & RegexOptions.IgnorePatternWhitespace);

            //추출
            MatchCollection KorClassesMc = ExtKorClasses.Matches(_html);

            //일치하지 않는 단어
            if (KorClassesMc.Count == 0)
            {
                richTextBox1.Text = "일치하는 단어가 없습니다";
                return;
            }

            //여러 단어 뜻을 배열에 저장
            string[] Words = new string[KorClassesMc.Count];
            for (int i = 0; i < Words.Length; i++)
            {
                Words[i] = KorClassesMc[i].Groups[0].Value;
            }

            //특수문자 제거
            for (int i = 0; i < Words.Length; i++)
                Words[i] = Regex.Replace(Words[i], "[^ㄱ-힗 ,]", String.Empty, RegexOptions.Singleline);

            //다듬는 작업
            for (int i = 1; i < 12; i++)
            {
                MatchCollection __mc = ExtKor.Matches(Words[i]);
                if (__mc.Count != 0)
                {
                    for (int j = 0; j < PartOfSpeech.Length; j++)
                    {
                        //품사이면 줄바꿔줌
                        if (Words[i] == PartOfSpeech[j])
                        {
                            resultstring += Words[i] + "\n";
                            break;
                        }
                        else if (Regex.Match(Words[i], "건").Success || Regex.Match(Words[i], ",건").Success)
                        {
                            resultstring += string.Empty;
                        }
                        else
                        {
                            resultstring += Words[i] + " ";
                            break;
                        }
                    }
                    resultstring += "\n";
                }

                else
                {
                    resultstring += string.Empty;
                }
            }
            //단어 넣어줌
            richTextBox1.Text = resultstring;
        }

        private void getEngWords(string _html)
        {
            Regex EngListTag = new Regex("<dl class=\"list_e2\">.+</dl>", RegexOptions.Singleline);
            Regex _EngListTag = new Regex("<span class=\"fnt_k0[0-9]\">.+</span>", RegexOptions.Singleline);

            Match EngListTagCollection = EngListTag.Match(_html);
            Match _EngListTagCollection = _EngListTag.Match(EngListTagCollection.Value);

            string resultString = _EngListTagCollection.Value;
            resultString = System.Text.RegularExpressions.Regex.Replace(resultString, "<[^>]*>", "");
            resultString = System.Text.RegularExpressions.Regex.Replace(resultString, "\r", "");
            resultString = System.Text.RegularExpressions.Regex.Replace(resultString, "\n", "");
            resultString = System.Text.RegularExpressions.Regex.Replace(resultString, "\t", "");

            //SourceViewer.HtmlSource = resultString;
            //SourceViewer sv = new SourceViewer();
            //sv.Show();

            richTextBox1.Text = resultString;
        }

        //Html소스 가져오기
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Focused && (e.KeyCode == Keys.Enter))
            {
                string SearchWord = textBox1.Text;

                Uri WordUri = new Uri("http://endic.naver.com/search.nhn?sLn=kr&isOnlyViewEE=N&query=" + SearchWord);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(WordUri);

                try
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader responseStreamReader = new StreamReader(responseStream);

                    string responseString = responseStreamReader.ReadToEnd();

                    //입력된 단어의 언어 판별
                    if (IsOnlyEnglish(SearchWord))
                        getKorWords(responseString);
                    else if (IsOnlyKor(SearchWord))
                        getEngWords(responseString);
                    else
                        richTextBox1.Text = "일치하는 단어가 없습니다";

                    responseStream.Close();
                    responseStreamReader.Close();
                }
                catch
                {
                    richTextBox1.Text = "일치하는 단어가 없습니다";
                }
            }
        }

        //포커스 가지면 텍스트박스 안에 있는 글자 비워주기
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        #region drag
        private Point mCurrentPosition = new Point(0, 0);
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                mCurrentPosition = new Point(-e.X, -e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    this.Location.X + (mCurrentPosition.X + e.X),
                    this.Location.Y + (mCurrentPosition.Y + e.Y));// 마우스의 이동치를 Form Location에 반영한다.
            }
        }
        #endregion

        //완전히 종료
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] ProcessList = Process.GetProcessesByName("WindowsFormsApplication");
            if (ProcessList.Length > 0)
                ProcessList[0].Kill();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        /// <summary>
        /// 알파벳으로 된 영문 문자열인지 여부를 알아내는 함수
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns>맞으면 true 틀리면 false</returns>
        public bool IsOnlyEnglish(string KeyWord)
        {
            StringBuilder stringBuilder = new StringBuilder(KeyWord);
            for (int i = 0; i < KeyWord.Length; i++)
            {
                string a = char.GetUnicodeCategory(stringBuilder[i]).ToString();
                if (a == "OtherLetter") 
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 한글로 된 한글 문자열인지 여부를 알아내는 함수
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns>맞으면 true 틀리면 false</returns>
        public bool IsOnlyKor(string KeyWord)
        {
            foreach(char KeyWordCharacter in KeyWord)
            {
                if ((Regex.Match(KeyWordCharacter.ToString(), "[ㄱ-힗]")).Length == 0)
                    return false;
            }
            return true;
        }

        private void minimize_button_MouseEnter(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(minimize_button, "최소화");
        }

        private void close_button_MouseEnter(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(close_button, "종료");
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            MinimizedButton mb = new MinimizedButton();

            Rectangle workingArea = Screen.GetWorkingArea(this);
            mb.Visible = true;
            mb.Location = new Point(0, workingArea.Bottom - mb.Size.Height);
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Process[] ProcessList = Process.GetProcessesByName("WindowsFormsApplication");
            if (ProcessList.Length > 0)
                ProcessList[0].Kill();

            this.Close();
        }

        private void SetTopLeft()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(0, workingArea.Top);
        }

        private void SetTopRight()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width, workingArea.Top);
        }

        private void SetBottomLeft()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(0, workingArea.Bottom - Size.Height);
        }

        private void SetBottomRight()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width, workingArea.Bottom - Size.Height);
        }
    }
}