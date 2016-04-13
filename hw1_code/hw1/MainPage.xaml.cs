using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace hw1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private delegate string AnimalSaying(object sender);//声明一个委托
        private event AnimalSaying Say;//委托声明一个事件

        interface Animal
        {
            //方法
            string saying(object sender);
            //属性
            int A { get; set; }
        }

        class cat : Animal
        {
            TextBlock word;
            private int a;
            public cat(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender)
            {
                this.word.Text = "I am a cat";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class dog : Animal
        {
            TextBlock word;
            private int a;
            public dog(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender)
            {
                this.word.Text = "I am a dog";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class pig : Animal
        {
            TextBlock word;
            private int a;
            public pig(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender)
            {
                this.word.Text = "I am a pig";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        private cat c;
        private dog d;
        private pig p;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            switch (textBox.Text)
            {
                case "pig":
                    Say = new AnimalSaying(p.saying);
                    Say(p);
                    break;
                case "dog":
                    Say = new AnimalSaying(d.saying);
                    Say(d);
                    break;
                case "cat":
                    Say = new AnimalSaying(c.saying);
                    Say(c);
                    break;
                default:
                    flag = false;
                    break;
            }
            if (flag)
            {
                words.Text = textBox.Text + ":" + words.Text;
            } else
            {
                words.Text = "";
            }
            textBox.Text = "";
            flag = true;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            words.Text = "";
            c = new cat(words);
            d = new dog(words);
            p = new pig(words);
            Random rd = new Random();
            int num = rd.Next(0, 3);
            switch (num)
            {
                case 0:
                    Say = new AnimalSaying(p.saying);
                    Say(p);
                    break;
                case 1:
                    Say = new AnimalSaying(d.saying);
                    Say(d);
                    break;
                case 2:
                    Say = new AnimalSaying(c.saying);
                    Say(c);
                    break;
            }
        }
    }
}
