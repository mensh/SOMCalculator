using DynamicData;
using DynamicData.Binding;
using Jot;
using Microsoft.VisualBasic;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.parsertokens;
using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMCalculator.ViewModels
{
    public class MianWindowViewModel: ReactiveObject
    {
        private Expression _ex;
        private string _text;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }


        private string _resultHex;
        public string ResultHex
        {
            get => _resultHex;
            set => this.RaiseAndSetIfChanged(ref _resultHex, value);
        }

        private string _resultBin;
        public string ResultBin
        {
            get => _resultBin;
            set => this.RaiseAndSetIfChanged(ref _resultBin, value);
        }

        private string _resultOct;
        public string ResultOct
        {
            get => _resultOct;
            set => this.RaiseAndSetIfChanged(ref _resultOct, value);
        }

        private string _hexFloat;
        public string HexFloat
        {
            get => _hexFloat;
            set => this.RaiseAndSetIfChanged(ref _hexFloat, value);
        }

        private string _hexInt;
        public string HexInt
        {
            get => _hexInt;
            set => this.RaiseAndSetIfChanged(ref _hexInt, value);
        }

        private HistoryItems _selectedValue;
        public HistoryItems SelectedValue
        {
            get => _selectedValue;
            set => this.RaiseAndSetIfChanged(ref _selectedValue, value);
        }
        private ObservableCollection<HistoryItems> _history;
        public ObservableCollection<HistoryItems> History
        {
            get => _history;
            set => this.RaiseAndSetIfChanged(ref _history, value);
        }
        public ReactiveCommand<object, System.Reactive.Unit> BitClickCommand { get; set; }
        public ReactiveCommand<System.Windows.Input.Key, System.Reactive.Unit> KeyEnterCommand { get; set; }

        public ObservableCollection<DataBit> DataBits { get; set; }


        private void ClarBits()
        {
            foreach (var bit in DataBits)
            {
                bit.Value = false;
            }
        }

        public MianWindowViewModel()
        {
            History = new ObservableCollection<HistoryItems>();
            Tracker tracker = Locator.Current.GetService<Tracker>();
            tracker.Configure<MianWindowViewModel>()
           .Properties(w => new
           {
               w.History
           });
            tracker.Track(this);

            BitClickCommand = ReactiveCommand.Create<object>(BitClick);
            KeyEnterCommand = ReactiveCommand.Create<System.Windows.Input.Key>(KeyEnter);
            DataBits = new ObservableCollection<DataBit>();
            for (int i=31; i>=0; i--)
            {
                DataBits.Add(new DataBit() { Name = i.ToString() });
            }

            DataBits.ToObservableChangeSet().WhenValueChanged(x => x.Value).Subscribe(x =>
            {
                Debug.WriteLine(x);
            });


            this.WhenAnyValue(x => x.Text).Throttle(TimeSpan.FromMilliseconds(10), RxApp.MainThreadScheduler).Subscribe(x =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    Result = "error";
                    ResultHex = "error";
                    ResultBin = "error";
                    ResultOct = "error";
                    HexFloat = "error";
                    HexInt = "error";
                    ClarBits();
                    return;
                }
                if (x.Contains("<<"))
                {
                    x = x.Replace("<<", "@<<");
                }
                if (x.Contains(">>"))
                {
                    x = x.Replace(">>", "@>>");
                }
                if (x.Contains("|"))
                {
                    x = x.Replace("|", "@|");
                }
                if (x.Contains("^"))
                {
                    x = x.Replace("^", "@^");
                }
                if (x.Contains("~"))
                {
                    x = x.Replace("~", "@~");
                }
                if (x.Contains("&"))
                {
                    x = x.Replace("&", "@&");
                }
                if (x.Contains("0x"))
                {
                    x = x.Replace("0x", "h.");
                }
                if (x.Contains("b"))
                {
                    x = x.Replace("b", "b.");
                }
                if (x.Contains("o"))
                {
                    x = x.Replace("o", "o.");
                }


                _ex = new Expression(x);
                if (_ex.checkSyntax())
                {

                    var result = _ex.calculate();

                    Result = result.ToString().Replace(",",".");
                    ResultHex = StringUtils.numberToHexString(result);

                    ResultBin = ToBinary((int)result);
                    ResultOct = Convert.ToString((int)result, 8);
                    HexFloat = BitConverter.ToString(BitConverter.GetBytes((float)result)).Replace("-", "");
                    HexInt = BitConverter.ToString(BitConverter.GetBytes((int)result)).Replace("-", "");
                }
                else
                {
                    Result = "error";
                    ResultHex = "error";
                    ResultBin = "error";
                    ResultOct = "error";
                    HexFloat = "error";
                    HexInt = "error";
                    ClarBits();
                }

            });
        }

        private void KeyEnter(System.Windows.Input.Key e)
        {
            if (_ex.checkSyntax())
            {
                var item = new HistoryItems() { Value = Text };
                
                History.Insert(0, item);
                if (History.Count>20)
                {
                    History.Remove(History.Last());
                }
            }
        }

        private void BitClick(object arg)
        {
            var intValue = getIntFromBitArray(DataBits.Select(x=>x.Value).Reverse().ToArray());
            Text = intValue.ToString();
        }


        private Int64 getIntFromBitArray(bool[] bitArray)
        {
            Int64 value = 0;

            for (int i = 0; i < bitArray.Count(); i++)
            {
                if (bitArray[i])
                    value += Convert.ToInt64(Math.Pow(2, i));
            }

            return value;
        }


        public  string ToBinary(int x)
        {
            char[] buff = new char[32];

            for (int i = 31; i >= 0; i--)
            {
                int mask = 1 << i;
                buff[31 - i] = (x & mask) != 0 ? '1' : '0';
                DataBits[31-i].Value = (x & mask) != 0 ? true : false;
            }

            return new string(buff);
        }
    }
}
