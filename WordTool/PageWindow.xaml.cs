using CastmRepository;
using JinShanSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WordTool
{
    /// <summary>
    /// Interaction logic for PageWindow.xaml
    /// </summary>
    public partial class PageWindow : Window
    {
        public WordRepository wordRepository = null;

        public MediaPlayer player = new MediaPlayer();

        public BindingList<GroupDomain> GroupListForDropdownList { get; set; }
        public BindingList<ModelDomain> ModelListForDropdownList { get; set; }

        public static SentenceDomain SentenceDomain { get; set; }

        public string TableName { get; set; }

        public ModelEnum ModelFlag { get; set; }

        public string SelectedValue { get; set; }

        public int CurrentValue { get; set; }

        public ModelDomain SelectedModelDomain { get; set; }

        public WordDomain CurrentWordDomain { get; set; }

        public WordDomain LastWrodDomain { get; set; }

        private System.Timers.Timer timer = new System.Timers.Timer() { AutoReset = false, Interval = 1000 };

        public PageWindow(string tableName)
        {
            InitializeComponent();
            InitDropdownList();
            this.TableName = tableName;
            this.wordRepository = new WordRepository(this.TableName);
            this.timer.Elapsed += new ElapsedEventHandler(this.GoNextByTimer);
            this.InitSentence();
            this.InitPage();
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.Submit_Click(null, null);
        //}

        private void InitSentence()
        {
            if (SentenceDomain == null)
            {
                SentenceDomain = JinShanUtil.GetSentence();
            }
            var sentenceStr = SentenceDomain.content + "\r\n" + SentenceDomain.note;
            this.EveryDay.Content = sentenceStr;
            this.SentencePic.Source = new BitmapImage(new Uri(SentenceDomain.picture));
        }

        private void InitDropdownList()
        {
            this.GroupListForDropdownList = new BindingList<GroupDomain>();
            var list = new List<GroupDomain>
            {
                new GroupDomain
                {
                    Name = "10"
                },
                new GroupDomain
                {
                    Name = "20"
                },
                new GroupDomain
                {
                    Name = "30"
                },
                new GroupDomain
                {
                    Name = "40"
                },
                new GroupDomain
                {
                    Name = "50"
                }
            };
            list.ForEach(p => this.GroupListForDropdownList.Add(p));
            this.DropDownList.SelectedItem = this.GroupListForDropdownList.FirstOrDefault();

            this.ModelListForDropdownList = new BindingList<ModelDomain>();
            var modelList = new List<ModelDomain>
            {
                new ModelDomain
                {
                    Model = ModelEnum.Default,
                    Value = "Default"
                },
                new ModelDomain
                {
                    Model = ModelEnum.Changed,
                    Value = "Changed"
                },
                new ModelDomain
                {
                    Model = ModelEnum.Mixed,
                    Value = "Mixed"
                },
                new ModelDomain
                {
                    Model = ModelEnum.Revised,
                    Value = "Revised"
                },
                new ModelDomain
                {
                    Model = ModelEnum.RevisedChange,
                    Value = "RevisedChange"
                }
            };
            modelList.ForEach(p => this.ModelListForDropdownList.Add(p));
            this.DropDownList_Model.SelectedItem = this.ModelListForDropdownList.FirstOrDefault();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow pageWindow = new MainWindow();
            pageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //pageWindow.Left = 200;
            //pageWindow.Top = 200;
            pageWindow.Show();
            this.Close();
        }

        private void Rest_Click(object sender, RoutedEventArgs e)
        {

            if (System.Windows.MessageBox.Show("are you sure to make the source as unread ? ", "Popup：", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ThreadPool.QueueUserWorkItem((p) => { this.wordRepository.ResetFlag(); });
            }
        }

        private void comboBoxInWnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxInWnd_SelectionChanged_Model(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void RadioButton_CheckDefault(object sender, RoutedEventArgs e)
        //{
        //    this.DefaultModel.IsChecked = true;
        //    this.MixedModel.IsChecked = false;
        //    this.Revise.IsChecked = false;
        //    this.ModelFlag = 0;
        //}

        //private void RadioButton_CheckMixed(object sender, RoutedEventArgs e)
        //{
        //    this.MixedModel.IsChecked = true;
        //    this.DefaultModel.IsChecked = false;
        //    this.Revise.IsChecked = false;
        //    this.ModelFlag = 1;
        //}

        //private void RadioButton_CheckRevise(object sender, RoutedEventArgs e)
        //{
        //    this.Revise.IsChecked = true;
        //    this.DefaultModel.IsChecked = false;
        //    this.MixedModel.IsChecked = false;
        //    this.ModelFlag = 2;
        //}

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOne = this.DropDownList.SelectedValue as GroupDomain;
            this.SelectedValue = selectedOne.Name.ToString();
            this.CurrentValue = 1;
            this.SelectedModelDomain = this.DropDownList_Model.SelectedItem as ModelDomain;
            this.TipsContent.Content = string.Empty;
            this.Yes.Visibility = Visibility.Hidden;
            this.No.Visibility = Visibility.Hidden;
            this.VoiceError.Content = string.Empty;
            this.Stack.Children.Clear();
            this.InitWordData();
        }

        private void Tip_Click(object sender, RoutedEventArgs e)
        {
            var tip = this.CurrentWordDomain.Phonetic;
            tip += "; ";
            if (this.CurrentWordDomain.TrascationFlag || this.SelectedModelDomain.Model == ModelEnum.Changed || this.SelectedModelDomain.Model == ModelEnum.RevisedChange)
            {
                tip += this.CurrentWordDomain.Word;
            }
            else
            {
                tip += this.CurrentWordDomain.Trascation;
            }
            this.TipsContent.Content = tip;
            this.Input.Focus();
        }

        private void InitPage()
        {
            //this.ModelFlag = false;
            //this.Yes.Visibility = Visibility.Hidden;
            //this.No.Visibility = Visibility.Hidden;
            this.SelectedValue = "10";
            this.SelectedModelDomain = new ModelDomain
            {
                Model = ModelEnum.Default,
                Value = "Default"
            };
            this.CurrentValue = 1;
            this.InitWordData();
        }

        private void InitWordData(WordDomain wordInfo = null)
        {
            var currentModel = this.SelectedModelDomain.Model;
            switch (currentModel)
            {
                case ModelEnum.Default:
                    wordInfo = wordInfo == null ? this.wordRepository.GetOne() : wordInfo;
                    if (wordInfo == null)
                    {
                        MessageBox.Show("resource is null, please import resource.");
                        return;
                    }
                    this.Word.Content = wordInfo.Word;
                    wordInfo.TrascationFlag = false;
                    break;
                case ModelEnum.Changed:
                    wordInfo = wordInfo == null ? this.wordRepository.GetOne() : wordInfo;
                    if (wordInfo == null)
                    {
                        MessageBox.Show("resource is null, please import resource.");
                        return;
                    }
                    this.Word.Content = wordInfo.Trascation;
                    wordInfo.TrascationFlag = false;
                    break;
                case ModelEnum.Mixed:
                    wordInfo = wordInfo == null ? this.wordRepository.GetOne() : wordInfo;
                    if (wordInfo == null)
                    {
                        MessageBox.Show("resource is null, please import resource.");
                        return;
                    }
                    Random rd = new Random();
                    var number = rd.Next(1000, 10000);
                    if (number % 2 == 0)
                    {
                        this.Word.Content = wordInfo.Word;
                        wordInfo.TrascationFlag = false;
                    }
                    else
                    {
                        this.Word.Content = wordInfo.Trascation;
                        wordInfo.TrascationFlag = true;
                    }
                    break;
                case ModelEnum.Revised:
                    wordInfo = wordInfo == null ? this.wordRepository.GetReadedOne() : wordInfo;
                    if (wordInfo == null)
                    {
                        MessageBox.Show("resource is null, please import resource.");
                        return;
                    }
                    this.Word.Content = wordInfo.Word;
                    wordInfo.TrascationFlag = false;
                    break;
                case ModelEnum.RevisedChange:
                    wordInfo = wordInfo == null ? this.wordRepository.GetReadedOne() : wordInfo;
                    if (wordInfo == null)
                    {
                        MessageBox.Show("resource is null, please import resource.");
                        return;
                    }
                    this.Word.Content = wordInfo.Trascation;
                    wordInfo.TrascationFlag = false;
                    break;
                default:
                    break;
            }
            this.CurrentWordDomain = wordInfo;
        }

        private void Input_Complete(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputValue = this.Input.Text;
                if (string.IsNullOrEmpty(inputValue))
                {
                    this.Tip_Click(null, null);
                    return;
                }
                var isCorrect = false;
                switch (this.SelectedModelDomain.Model)
                {
                    case ModelEnum.Default:
                    case ModelEnum.Revised:
                        //if (this.TableName.Contains("Custom"))
                        //{
                            //var wordTemp = this.ConvertTraslationToList(this.CurrentWordDomain.Trascation);
                            //isCorrect = wordTemp.Any(p => p.Contains(inputValue.ToLowerInvariant()));
                        //}
                        //else
                        //{
                            isCorrect = inputValue.Equals(this.CurrentWordDomain.Trascation, StringComparison.OrdinalIgnoreCase);
                        //}
                        break;
                    case ModelEnum.Changed:
                    case ModelEnum.RevisedChange:
                        isCorrect = this.CurrentWordDomain.Word.Contains(inputValue);
                        break;
                    case ModelEnum.Mixed:
                        if (this.CurrentWordDomain.TrascationFlag)
                        {
                            isCorrect = this.CurrentWordDomain.Word.Contains(inputValue);
                        }
                        else
                        {
                            isCorrect = inputValue.Equals(this.CurrentWordDomain.Trascation, StringComparison.OrdinalIgnoreCase);
                        }
                        break;
                    default:
                        break;
                }

                if (isCorrect)
                {
                    this.Yes.Visibility = Visibility.Visible;
                    var flag = this.SelectedModelDomain.Model.Equals(ModelEnum.Revised) || this.SelectedModelDomain.Model.Equals(ModelEnum.RevisedChange) ? 2 : 1;
                    this.wordRepository.UpdateFlagById(this.CurrentWordDomain.Id, flag);
                    this.CreateProcess();
                    this.timer.Start();
                }
                else
                {
                    this.No.Visibility = Visibility.Visible;
                }
            }
        }

        //private void Input_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        this.Yes.Visibility = Visibility.Hidden;
        //        this.No.Visibility = Visibility.Hidden;
        //    }
        //}

        private void GoNext(object sender, EventArgs e)
        {
            this.TipsContent.Content = string.Empty;
            this.Yes.Visibility = Visibility.Hidden;
            this.No.Visibility = Visibility.Hidden;
            this.VoiceError.Content = string.Empty;
            this.Input.Text = string.Empty;
            this.Input.Focus();

            var newOne = this.SelectedModelDomain.Model.Equals(ModelEnum.Revised) ||  this.SelectedModelDomain.Model.Equals(ModelEnum.RevisedChange) ? this.wordRepository.GetReadedOne() : this.wordRepository.GetOneExcludeById(this.CurrentWordDomain.Id);
            if (newOne != null)
            {
                //ThreadPool.QueueUserWorkItem((p => { this.Voice_Click(null, null); }));
                this.LastWrodDomain = this.CurrentWordDomain;
                this.CurrentWordDomain = newOne;
                this.Voice_Click(null, null);
                this.InitWordData(newOne);
            }
            else
            {
                MessageBox.Show("resource is null, please import resource.");
            }
            
        }

        private void GoBack(object sender, EventArgs e)
        {
            this.TipsContent.Content = string.Empty;
            this.Yes.Visibility = Visibility.Hidden;
            this.No.Visibility = Visibility.Hidden;
            this.VoiceError.Content = string.Empty;
            this.Input.Text = string.Empty;
            this.Input.Focus();

            this.CurrentWordDomain = this.LastWrodDomain;
            //ThreadPool.QueueUserWorkItem((p => { this.Voice_Click(null, null); }));
            this.Voice_Click(null, null);
            this.InitWordData(this.CurrentWordDomain);
        }

        private List<string> ConvertTraslationToList(string value)
        {
            var first = value.Split('{');
            var container = new List<string>();
            foreach (var secondItem in first)
            {
                var third = secondItem.Split('}');
                container.AddRange(third.Select(p => p).ToList());
            }
            container.Where(p => !string.IsNullOrEmpty(p)).ToList().ForEach(p=> {
                if (p.StartsWith(":"))
                {
                    p.TrimStart(';');
                    p.ToLowerInvariant();
                }
            });
            return container;
        }

        private void GoNextByTimer(object state, ElapsedEventArgs e)
        {
            this.Yes.Dispatcher.Invoke(new Action(() =>
            {
                this.GoNext(null, null);
                //this.Voice_Click(null, null);
            }));
        }

        private void CreateProcess()
        {
            if (this.CurrentValue == 1)
            {
                this.Stack.Children.Clear();
            }
            var width = 450 / Convert.ToInt32(this.SelectedValue);
            var marginLeft = width * this.CurrentValue;
            Border border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9000")),
                Height = 36,
                Width = width,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 0)
            };
            if (this.CurrentValue == 1)
            {
                border.CornerRadius = new CornerRadius(20, 0, 0, 20);
            } else if (this.CurrentValue == Convert.ToInt32(this.SelectedValue))
            {
                border.CornerRadius = new CornerRadius(0, 20, 20, 0);
            }
            else
            {
                border.CornerRadius = new CornerRadius(0, 0, 0, 0);
            }
            this.Stack.Children.Add(border);
            if (this.CurrentValue >= Convert.ToInt32(this.SelectedValue))
            {
                this.CurrentValue = 1;
            }
            else
            {
                this.CurrentValue++;
            }
        }

        private void Voice_Click(object sender, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem((p) =>
            //{
            var voiceUrl = string.Empty;
            var ph = string.Empty;
            if (!string.IsNullOrEmpty(this.CurrentWordDomain.Voice))
            {
                voiceUrl = this.CurrentWordDomain.Voice;
            }
            else
            {
                var info = JinShanUtil.GetVoiceUrl(this.CurrentWordDomain.Trascation);
                if (info != null)
                {
                    voiceUrl = info.VoiceUrl;
                    ph = info.PH;
                }
            }
            if (!string.IsNullOrEmpty(voiceUrl))
            {
                this.player.Open(new Uri(voiceUrl));
                this.player.Play();
                this.wordRepository.UpdateVoiceById(this.CurrentWordDomain.Id, voiceUrl, ph);
            }
            else
            {
                this.VoiceError.Content = "it has no voice";
            }
            //});
        }
    }
}
