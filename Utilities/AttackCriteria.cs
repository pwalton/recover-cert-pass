using System;
using System.Configuration;
using System.IO;

namespace RecoverCertPassword.Utilities
{
    public class AttackCriteria
    {
        private static AttackCriteria _attackCriteria;

        private string _certpath;
        private string[] _dictionaryItems = null;
        private string[] _dictionaryPrefixesToSkip = null;
        private string _dictionaryPath;
        private string _prefixesToSkipPath;
        private int _minimumPasswordLength = 0;
        private int _maximumPasswordLength = 0;
        private int _maxDictionaryItemsToUse = 0;
        private bool? _dictionaryItemsMayRepeat = null;

        public static AttackCriteria CurrentCriteria
        {
            get
            {
                if (_attackCriteria == null)
                    _attackCriteria = new AttackCriteria();

                return _attackCriteria;
            }
        }

        private AttackCriteria() {}

        public string CertificatePath
        {
            get
            {
                if (String.IsNullOrEmpty(_certpath))
                {
                    _certpath = GetAppSettingFromKey<string>("CertificatePath");

                    if (!File.Exists(Path.GetFullPath(_certpath)))
                        throw new FileNotFoundException(
                            String.Format("Error finding file specified by CertificatePath value: {0}", _certpath));
                }
                return _certpath;
            }
        }

        public string[] DictionaryItems
        {
            get
            {
                if (_dictionaryItems == null)
                {
                    _dictionaryPath = Path.GetFullPath(GetAppSettingFromKey<string>("DictionaryPath"));
                    if (!File.Exists(_dictionaryPath))
                        throw new FileNotFoundException(
                            String.Format("Error finding file specified by DictionaryPath value: {0}", _dictionaryPath));

                    _dictionaryItems = File.ReadAllLines(_dictionaryPath);

                    if (_dictionaryItems.Length == 0)
                        throw new Exception(
                            String.Format("Error reading file specific by DictionaryPath: {0}", _dictionaryPath));
                }
                return _dictionaryItems;
            }
        }

        public string[] DictionaryPrefixesToSkip
        {
            get
            {
                if (_dictionaryPrefixesToSkip == null)
                {
                    _prefixesToSkipPath = Path.GetFullPath(GetAppSettingFromKey<string>("DictionaryPrefixesToSkip"));
                    if (String.IsNullOrWhiteSpace(_prefixesToSkipPath))
                        _dictionaryPrefixesToSkip = new string[] { };
                    else if (!File.Exists(_prefixesToSkipPath))
                        throw new FileNotFoundException(
                            String.Format("Error finding file specified by DictionaryPrefixesToSkip value: {0}", _prefixesToSkipPath));
                    else
                        _dictionaryPrefixesToSkip = File.ReadAllLines(_prefixesToSkipPath);
                }
                return _dictionaryPrefixesToSkip;
            }
        }

        public int MimimumPasswordLength
        {
            get
            {
                if (_minimumPasswordLength == 0)
                {
                    _minimumPasswordLength = GetAppSettingFromKey<int>("MinimumPasswordLength");
                }
                return _minimumPasswordLength;
            }
        }

        public int MaximumPasswordLength
        {
            get
            {
                if (_maximumPasswordLength == 0)
                {
                    _maximumPasswordLength = GetAppSettingFromKey<int>("MaximumPasswordLength");
                }
                return _maximumPasswordLength;
            }
        }

        public int MaxDictionaryItemsToUse
        {
            get
            {
                if (_maxDictionaryItemsToUse == 0)
                {
                    _maxDictionaryItemsToUse = GetAppSettingFromKey<int>("MaximumDictionaryItemsToUse");
                }
                return _maxDictionaryItemsToUse;
            }
        }

        public bool DictionaryItemsMayRepeat
        {
            get
            {
                if (!_dictionaryItemsMayRepeat.HasValue)
                {
                    _dictionaryItemsMayRepeat = GetAppSettingFromKey<bool>("DictionaryItemsMayRepeat");
                }
                return _dictionaryItemsMayRepeat.Value;
            }
        }

        public override string ToString()
        {
            return String.Format(
                "Target Certificate: {0}\n" +
                "Dictionary items: {1}\n" +
                "Dictionary prefixes to skip: {2}\n" +
                "Dictionary items may repeat: {3}\n" +
                "Password length: {4} to {5} characters\n" +
                "Maximum number of dictionary items to use: {6}",
                CertificatePath,
                String.Join(",", DictionaryItems),
                String.Join(",", DictionaryPrefixesToSkip),
                DictionaryItemsMayRepeat,
                MimimumPasswordLength, 
                MaximumPasswordLength,
                MaxDictionaryItemsToUse
            );
        }

        private T GetAppSettingFromKey<T>(string key)
        {
            T appSettingValue = default(T);
            appSettingValue = (T)Convert.ChangeType(ConfigurationManager.AppSettings.Get(key), typeof(T));
            if (appSettingValue.Equals(null) || appSettingValue.Equals(""))
                throw new Exception(String.Format("Error reading {0} from config", key));
            return appSettingValue;
        }
    }

    public static class SYSTEM_ERROR_CODES
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_INVALID_DATA = 13;
    }
}
