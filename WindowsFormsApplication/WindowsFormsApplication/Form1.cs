using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;

/// <summary>
/// Eng to Kor 
/// Kor to Eng (new Regex("<span class = "fnt_e[0-9]>.+</span>");
/// </summary>

namespace WindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private string resultstring { get; set; }
        private string[] PartOfSpeech = { "명사", "대명사", "형용사", "동사", "부사", "전치사", "접속사", "감탄사" };

        public string WordForSearch { get; set; }
        public Uri WordForUrl { get; set; }

        private HttpWebRequest httpWebRequest;
        private HttpWebResponse httpWebResponse;

        private Stream responseStream;
        private StreamReader responseStreamReader;

        private string responseString;

        private Point mCurrentPosition = new Point(0, 0);

        public Form1()
        {
            InitializeComponent();
            
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(0, workingArea.Bottom - Size.Height);

            this.TopMost = true;
            //this.Width = 19;

            button1.FlatAppearance.BorderSize = 0;

            trackBar1.Value = 100;

        }

        private void getWords(string _html)
        {
            resultstring = null;

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
            {
                Words[i] = Regex.Replace(Words[i], "[^ㄱ-힗 ,]", String.Empty, RegexOptions.Singleline);
            }

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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Focused && (e.KeyCode == Keys.Enter))
            {
                WordForSearch = textBox1.Text;

                WordForUrl = new Uri("http://endic.naver.com/search.nhn?sLn=kr&isOnlyViewEE=N&query=" + WordForSearch);
                httpWebRequest = (HttpWebRequest)WebRequest.Create(WordForUrl);

                try
                {
                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    responseStream = httpWebResponse.GetResponseStream();
                    responseStreamReader = new StreamReader(responseStream);

                    responseString = responseStreamReader.ReadToEnd();
                    
                    getWords(responseString);
                    responseStream.Close();
                    responseStreamReader.Close();
                }
                catch 
                {
                    richTextBox1.Text = "네트워크에 연결되어 있지 않습니다.";
                }
                finally
                {
                    responseStream.Close();
                    responseStreamReader.Close();
                    httpWebResponse.Dispose();
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }

        #region drag
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

        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(0,  workingArea.Bottom - Size.Height);
        }

        private bool IsMinimized = false;
        private void button1_Click(object sender, EventArgs e)
        {/*
            if (!IsMinimized)
            {
                this.Width = 19;
                
                IsMinimized = true;
               
                Rectangle workingArea = Screen.GetWorkingArea(this);
                this.Location = new Point(0, workingArea.Bottom - Size.Height);
            }
            else
            {
                this.Width = 462;

                IsMinimized = false;
                
                Rectangle workingArea = Screen.GetWorkingArea(this);
                this.Location = new Point(0,  workingArea.Bottom - Size.Height); 
            }*/

            this.Visible = false;
            MinimizedButton mb = new MinimizedButton();
            
            Rectangle workingArea = Screen.GetWorkingArea(this);
            mb.Visible = true;
            mb.Location = new Point(0, workingArea.Bottom - mb.Size.Height);
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button1, "버튼을 클릭해주세요 :)");
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            this.Opacity = trackBar1.Value / 100f;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] ProcessList = Process.GetProcessesByName("WindowsFormsApplication");
            if (ProcessList.Length > 0)
                ProcessList[0].Kill();
        }
    }
    #region KorToEng
    #endregion
}