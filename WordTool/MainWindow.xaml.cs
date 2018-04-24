using BaiduPackage;
using CastmExcel;
using CastmRepository;
using JinShanSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly string historyResourceUrl = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf("\\bin")) + "\\ResourceOpration\\Resource\\OtherResource.resx";
        private static readonly string ChineseRule = @"[\u4e00-\u9fa5]";
        private static readonly string JapaneseRule = @"[\u0800-\u4e00]";
        private static readonly int historyCount = 7;
        private static Dictionary<string, string> mappingDic = ResourceAction.GetTableNameDictionary();
        private string TableName { get; set; }

        public static List<string> HistoryList = new List<string>();

        public ExcelUtil ExcelUtil = new ExcelUtil();

        public WordRepository wordRepository = null;

        public MainWindow()
        {
            InitializeComponent();
            this.InitDropdownList(LanguageEnum.EN);
            this.InitHistoryContent();
            this.InitDataBase();
        }

        public BindingList<LanguageDomain> LanguageListForDropdownList { get; set; }

        public static List<LanguageDomain> LanguageListCache = new List<LanguageDomain>();

        public static List<LanguageDomain> LanguageList
        {
            get
            {
                if (!LanguageListCache.Any())
                {
                    InitLanguageListCache();
                    return LanguageListCache;
                }
                return LanguageListCache;
            }
            set
            {
                LanguageListCache = value;
            }
        }

        private static void InitLanguageListCache()
        {
            if (!LanguageListCache.Any())
            {
                var result = new List<LanguageDomain>
                {
                    new LanguageDomain
                    {
                        Name="TOEIC",
                        SourceName=string.Empty,
                        Type= LanguageEnum.EN
                    },
                    new LanguageDomain
                    {
                        Name="CET6",
                        SourceName=string.Empty,
                        Type= LanguageEnum.EN
                    },
                    new LanguageDomain
                    {
                        Name = "CET4",
                        SourceName = string.Empty,
                        Type = LanguageEnum.EN
                    },
                    new LanguageDomain
                    {
                        Name = "Level1",
                        SourceName = string.Empty,
                        Type = LanguageEnum.JP
                    },
                    new LanguageDomain
                    {
                        Name = "Level2",
                        SourceName = string.Empty,
                        Type = LanguageEnum.JP
                    },
                    new LanguageDomain
                    {
                        Name="Level3",
                        SourceName = string.Empty,
                        Type = LanguageEnum.JP
                    },
                    new LanguageDomain
                    {
                        Name = "Custom",
                        SourceName = string.Empty,
                        Type = LanguageEnum.OTHER
                    },
                    new LanguageDomain
                    {
                        Name="Other",
                        SourceName = string.Empty,
                        Type = LanguageEnum.OTHER
                    }
                };
                LanguageListCache = result;
            }
        }

        private static List<LanguageDomain> AddLanguageListCache(LanguageDomain languageDomain)
        {
            LanguageListCache.Add(languageDomain);
            return LanguageListCache;
        }

        private void InitDropdownList(LanguageEnum languageEnum)
        {
            if (this.LanguageListForDropdownList != null && this.LanguageListForDropdownList.Any())
            {
                this.LanguageListForDropdownList.Clear();
            }
            var options = LanguageList.Where(p => p.Type == languageEnum).ToList();
            if (options.Any())
            {
                this.LanguageListForDropdownList = this.LanguageListForDropdownList ?? new BindingList<LanguageDomain>();
                options.ForEach(p => { this.LanguageListForDropdownList.Add(p); });
            }
            //this.DropDownList.SelectedValue = options.FirstOrDefault().Name;
            this.DropDownList.SelectedItem = this.LanguageListForDropdownList.FirstOrDefault();
        }

        private void RadioButton_CheckEnglish(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButon_English = this.FindName("English") as RadioButton;
                radioButon_English.IsChecked = true;
                var radioButton_Japanese = this.FindName("Japanese") as RadioButton;
                radioButton_Japanese.IsChecked = false;
                var radioButton_Custom = this.FindName("Custom") as RadioButton;
                radioButton_Custom.IsChecked = false;

                this.InitDropdownList(LanguageEnum.EN);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RadioButton_CheckJanpanese(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButton_Japanese = sender as RadioButton;
                radioButton_Japanese.IsChecked = true;
                var radioButton_Custom = this.FindName("Custom") as RadioButton;
                radioButton_Custom.IsChecked = false;
                var radioButton_Engilish = this.FindName("English") as RadioButton;
                radioButton_Engilish.IsChecked = false;

                this.InitDropdownList(LanguageEnum.JP);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RadioButton_Custom(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButton_Custom = sender as RadioButton;
                radioButton_Custom.IsChecked = true;
                var radioButton_Japanese = this.FindName("Japanese") as RadioButton;
                radioButton_Japanese.IsChecked = false;
                var radioButton_Engilish = this.FindName("English") as RadioButton;
                radioButton_Engilish.IsChecked = false;

                this.InitDropdownList(LanguageEnum.OTHER);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //var homePage = new Uri("PlayPage.xaml", UriKind.Relative);
            //NavigationWindow window = new NavigationWindow();
            //window.Show();
            //var page = new PageWindow() ;

            //page.MainContent = this.Content;
            //this.Content = page.Content;

            //NavigationWindow window = new NavigationWindow();
            //window.Source = new Uri("PlayPage.xaml", UriKind.Relative);
            //window.Show();

            var seletedOne = this.DropDownList.SelectedValue as LanguageDomain;
            this.TableName = mappingDic[seletedOne.Name]; 

            PageWindow pageWindow = new PageWindow(this.TableName);
            pageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //pageWindow.Left =200;
            //pageWindow.Top = 200;
            pageWindow.Show();
            this.Close();
        }

        private void comboBoxInWnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox box = sender as ComboBox;

            //this.stackPanel1.DataContext = null;
            //this.stackPanel1.DataContext = box.SelectedItem;
        }

        private void Word_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SearchButton_Click(sender, e);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.wordRepository == null)
            {
                var seletedOne = this.DropDownList.SelectedValue as LanguageDomain;
                this.TableName = mappingDic[seletedOne.Name];
                this.wordRepository = new WordRepository(this.TableName);
            }
            var textBox = this.Word as TextBox;
            if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
            {
                this.TranslatedInformation.Content = string.Empty;
                if (!this.CheckIfInvalidInput(textBox.Text))
                {
                    TranslationResult result = null;
                    JinShanDomain jinshanDomain = null;
                    if (!textBox.Text.Any(p => !Regex.IsMatch(p.ToString(), ChineseRule)))
                    {
                        //result = TranslationMethod.TranslatedWithBaidu(textBox.Text, LanguageEnum.ZH, LanguageEnum.EN);
                        jinshanDomain = JinShanUtil.Translate(textBox.Text);
                    }
                    else if (!textBox.Text.Any(p => !Regex.IsMatch(p.ToString(), JapaneseRule)))
                    {
                        result = TranslationMethod.TranslatedWithBaidu(textBox.Text, LanguageEnum.JP, LanguageEnum.ZH);
                    }
                    else
                    {
                        result = TranslationMethod.TranslatedWithBaidu(textBox.Text, LanguageEnum.Auto, LanguageEnum.ZH);
                    }
                    //|| (!string.IsNullOrEmpty(result.Error_code) && result.Error_code != "52000")
                    if (result == null && jinshanDomain == null)
                    {
                        this.TranslatedInformation.Content = string.Format("Error code: {0}, {1}", result.Error_code, result.Error_msg);
                        this.TranslatedInformation.Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (jinshanDomain != null)
                        {
                            var wordInfo = new WordDomain
                            {
                                Id = Guid.NewGuid(),
                                Word = jinshanDomain.word_name,
                                Flag = 0,
                                CreateTime = DateTime.Now.Ticks
                            };
                            var defaultOne = jinshanDomain.symbols.FirstOrDefault();
                            wordInfo.Phonetic = defaultOne.ph_en;
                            wordInfo.Voice = this.ConvertVoice(defaultOne);
                            //var translationStr = string.Empty;
                            //foreach (var part in defaultOne.parts)
                            //{
                            //    var meanStr = string.Empty;
                            //    foreach (var mean in part.means)
                            //    {
                            //        meanStr += mean;
                            //        meanStr += ";";
                            //    }
                            //    translationStr += "{";
                            //    translationStr += part.part;
                            //    translationStr += ":";
                            //    translationStr += meanStr;
                            //    translationStr += "}";
                            //}
                            wordInfo.Trascation = defaultOne.parts.FirstOrDefault().means[0];
                            this.wordRepository = new WordRepository("Language_Custom");
                            this.wordRepository.InsertAll(new List<WordDomain> { wordInfo }, !string.IsNullOrEmpty(wordInfo.Voice));
                            // 删除缓存中最旧的data
                            this.UpdateHistoryResource(wordInfo.Word, string.Join(",", defaultOne.parts.FirstOrDefault().means.FirstOrDefault()));
                        }
                        else
                        {
                            var contentStr = string.Empty;
                            for (var i = 0; i < result.Trans_result.Length; i++)
                            {
                                if (i > 0)
                                {
                                    contentStr += "/r/n";
                                }
                                var line = string.Format("{0}:  {1}", result.Trans_result[i].Src, result.Trans_result[i].Dst);
                                contentStr += line;
                                // 添加到OtherResource
                                //ResourceAction.RemoveAndAddNewResEntry(historyResourceUrl, "Key_" + result.Trans_result[i].Src, result.Trans_result[i].Dst);
                                var wordInfo = new WordDomain()
                                {
                                    Id = Guid.NewGuid(),
                                    Word = result.Trans_result[i].Src,
                                    Trascation = result.Trans_result[i].Dst,
                                    Phonetic = result.Trans_result[i].Src,
                                    Flag = 0,
                                    CreateTime = DateTime.Now.Ticks
                                };
                                this.wordRepository.InsertAll(new List<WordDomain> { wordInfo });

                                // 删除缓存中最旧的data
                                this.UpdateHistoryResource(result.Trans_result[i].Src, result.Trans_result[i].Dst);
                            }
                            this.TranslatedInformation.Content = contentStr;
                        }
                        
                        this.TranslatedInformation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999933"));
                        this.InitHistoryContent();
                    }
                }
                else
                {
                    this.TranslatedInformation.Content = string.Format("Error: invalid word, please input again.");
                    this.TranslatedInformation.Foreground = Brushes.Red;
                    this.Word.Focus();
                }
            }
        }

        private bool CheckIfInvalidInput(string value)
        {
            //Regex reg = new Regex("[^/[/]/?/*]+");
            //var result = reg.Match(value);
            return value.Any(p => p == '^' || p == '~' || p == '`' || p == '|' || p == ',' || p == '?' || p == ';' || p == '.');
        }

        private void InitHistoryContent()
        {
            if (!HistoryList.Any())
            {
                //var resourceUrl = System.Web.HttpContext.Current.Server.MapPath("/HistoryContentResource.resx");
                //using (ResXResourceReader reader = new ResXResourceReader(historyResourceUrl))
                //{
                //    var count = 0;
                //    foreach (DictionaryEntry entry in reader)
                //    {
                //        if (count == historyCount)
                //        {
                //            break;
                //        }
                //        var lineValue = string.Format("{0}:  {1}", entry.Key.ToString().Substring(entry.Key.ToString().IndexOf('_') + 1), entry.Value);
                //        HistoryList.Add(lineValue);
                //        count++;
                //    }
                //    reader.Close();
                //}
            }

            // 动态添加textblock到stackpanel中
            this.HistoryContent.Children.Clear();
            foreach (var item in HistoryList)
            {
                var textBlockInfo = this.CreateTextBoxDynamic(item);
                this.HistoryContent.Children.Add(textBlockInfo);
            }
        }

        private TextBlock CreateTextBoxDynamic(string value)
        {
            var textBlock = new TextBlock(new Run(value))
            {
                Height = 22,
                Width = 270,
                FontFamily = new FontFamily("Segoe Script"),
                FontStyle = FontStyles.Italic,
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };
            return textBlock;
        }

        private void UpdateHistoryResource(string key, string value)
        {
            while (HistoryList.Count >= historyCount)
            {
                HistoryList.RemoveAt(0);
            }
            HistoryList.Add(string.Format("{0}:  {1}", key, value));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var localPath = dialog.SelectedPath;
                    //var fileName = this.DropDownList.SelectedValue.ToString();
                    var fileName = "Language_Template";
                    var domain = TemplateInfo.CreateTemplateForExport(localPath, fileName);
                    this.ExcelUtil.ExcelDomain = domain;
                    this.ExcelUtil.DownloadTemplate();
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filepath = openFileDialog.FileName;
                var excelDomain = this.ExcelUtil.ImportTemplate(filepath);
                var title = excelDomain.TitleList.FirstOrDefault().Title;
                if (string.IsNullOrEmpty(title)|| !mappingDic.ContainsKey(title))
                {
                    MessageBox.Show("The template name is invalid");
                }
                else
                {
                    this.TableName = mappingDic[title];
                    this.wordRepository = new WordRepository(this.TableName);
                    var wordList = this.ConvertExcelDomainToWordDomain(excelDomain);
                    // save to db
                    var errorList = this.wordRepository.InsertAll(wordList);
                    if (errorList != null && errorList.Any())
                    {
                        this.AddErrorToExcelDomain(errorList, ref excelDomain);
                        this.ExcelUtil.ExcelDomain = excelDomain;
                        this.ExcelUtil.DownloadTemplate(true);
                    }
                }
            }
        }

        private List<WordDomain> ConvertExcelDomainToWordDomain(ExcelDomain excelDomain)
        {
            var result = new List<WordDomain>();
            excelDomain.BodyDomain.DataList.ForEach(p =>
            {
                var wordDomain = new WordDomain
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.Now.Ticks
                };
                p.ColumnList.ForEach(pro =>
                {
                    switch (pro.ColumnNumber)
                    {
                        case 1:
                            wordDomain.Word = pro.Value;
                            break;
                        case 2:
                            wordDomain.Trascation = pro.Value;
                            break;
                        case 3:
                            wordDomain.Phonetic = pro.Value;
                            break;
                        case 4:
                            wordDomain.Voice = pro.Value;
                            break;
                        default:
                            break;
                    }
                });
                result.Add(wordDomain);
            });
            return result;
        }

        private void AddErrorToExcelDomain(List<WordDomain> wordList,ref ExcelDomain excelDomain)
        {
            var errorRows = new List<RowDomain>();
            //excelDomain.BodyDomain.DataList.ForEach(p=> {
            //    if (p.ErrorList.Any())
            //    {
            //        p.ErrorList.ForEach(pro =>
            //        {
                        
            //        });
            //    }
            //});
            excelDomain.BodyDomain.DataList.ForEach(p =>
            {
                if (wordList.Select(obj => obj.Word).ToList().Any(item => item == p.ColumnList.FirstOrDefault().Value))
                {
                    errorRows.Add(p);
                }
            });

            excelDomain.BodyDomain.DataList = errorRows;
        }

        private void InitDataBase()
        {
            if (!mappingDic.Any())
            {
                SqliteBaseRepository baseRepository = new SqliteBaseRepository();
                baseRepository.CreateNewDatabase();
                baseRepository.ConnectToDatabase();
                baseRepository.CreateTable();

                ResourceAction.AddMapping();
                mappingDic = ResourceAction.GetTableNameDictionary();
            }
        }

        private string ConvertVoice(symbols symbols)
        {
            var voiceUrl = string.Empty;
            if (!string.IsNullOrEmpty(symbols.ph_en_mp3))
            {
                voiceUrl = symbols.ph_en_mp3;
                return voiceUrl;
            }
            if (!string.IsNullOrEmpty(symbols.ph_am_mp3))
            {
                voiceUrl = symbols.ph_am_mp3;
                return voiceUrl;
            }
            if (!string.IsNullOrEmpty(symbols.ph_tts_mp3))
            {
                voiceUrl = symbols.ph_tts_mp3;
                return voiceUrl;
            }
            if (!string.IsNullOrEmpty(symbols.symbol_mp3))
            {
                voiceUrl = symbols.symbol_mp3;
                return voiceUrl;
            }
            return voiceUrl;
        }
    }
}
