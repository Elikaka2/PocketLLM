using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PocketLLM.Config
{
    public class ConfigClass : INotifyPropertyChanged
    {
        private string _apiKey;
        public string ApiKey {
            get => _apiKey;
            set
            {
                _apiKey = value;
                OnPropertyChanged(nameof(ApiKey));
            }
        }
        private string _model;
        public string Model {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }
        private string _llmType;
        public string LLMType {
            get => _llmType;
            set
            {
                _llmType = value;
                OnPropertyChanged(nameof(LLMType));
            }
        }
        private bool _isAutoStart;
        public bool isAutoStart { 
            get => _isAutoStart;
            set 
            {
                _isAutoStart = value;
                OnPropertyChanged(nameof(isAutoStart));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
