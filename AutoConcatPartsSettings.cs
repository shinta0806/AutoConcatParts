// ============================================================================
// 
// Vegas 用動画切り貼りスクリプトの設定を管理
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using Shinta;
using System;
using System.Configuration;

namespace AutoConcatParts.Shared
{
	// 設定の保存場所を指定可能にする
	[SettingsProvider(typeof(VariableSettingsProvider))]
	public class AutoConcatPartsSettings : ApplicationSettingsBase
	{
		// ====================================================================
		// public プロパティ
		// ====================================================================

		// --------------------------------------------------------------------
		// 設定
		// --------------------------------------------------------------------

		// 先頭除外：分
		private const String KEY_NAME_HEAD_MIN = "HeadMin";
		[UserScopedSetting]
		public String HeadMin
		{
			get
			{
				return (String)this[KEY_NAME_HEAD_MIN];
			}
			set
			{
				this[KEY_NAME_HEAD_MIN] = value;
			}
		}

		// 先頭除外：秒
		private const String KEY_NAME_HEAD_SEC = "HeadSec";
		[UserScopedSetting]
		public String HeadSec
		{
			get
			{
				return (String)this[KEY_NAME_HEAD_SEC];
			}
			set
			{
				this[KEY_NAME_HEAD_SEC] = value;
			}
		}

		// 先頭除外：ミリ秒
		private const String KEY_NAME_HEAD_MILLI_SEC = "HeadMilliSec";
		[UserScopedSetting]
		public String HeadMilliSec
		{
			get
			{
				return (String)this[KEY_NAME_HEAD_MILLI_SEC];
			}
			set
			{
				this[KEY_NAME_HEAD_MILLI_SEC] = value;
			}
		}

		// 末尾除外：分
		private const String KEY_NAME_TAIL_MIN = "TailMin";
		[UserScopedSetting]
		public String TailMin
		{
			get
			{
				return (String)this[KEY_NAME_TAIL_MIN];
			}
			set
			{
				this[KEY_NAME_TAIL_MIN] = value;
			}
		}

		// 末尾除外：秒
		private const String KEY_NAME_TAIL_SEC = "TailSec";
		[UserScopedSetting]
		public String TailSec
		{
			get
			{
				return (String)this[KEY_NAME_TAIL_SEC];
			}
			set
			{
				this[KEY_NAME_TAIL_SEC] = value;
			}
		}

		// 末尾除外：ミリ秒
		private const String KEY_NAME_TAIL_MILLI_SEC = "TailMilliSec";
		[UserScopedSetting]
		public String TailMilliSec
		{
			get
			{
				return (String)this[KEY_NAME_TAIL_MILLI_SEC];
			}
			set
			{
				this[KEY_NAME_TAIL_MILLI_SEC] = value;
			}
		}

		// 時間で指定する
		private const String KEY_NAME_BY_TIME = "ByTime";
		[UserScopedSetting]
		[DefaultSettingValue(Common.BOOLEAN_STRING_TRUE)]
		public Boolean ByTime
		{
			get
			{
				return (Boolean)this[KEY_NAME_BY_TIME];
			}
			set
			{
				this[KEY_NAME_BY_TIME] = value;
			}
		}

		// 時間で指定する：秒
		private const String KEY_NAME_TIME_SEC = "TimeSec";
		[UserScopedSetting]
		[DefaultSettingValue("5")]
		public String TimeSec
		{
			get
			{
				return (String)this[KEY_NAME_TIME_SEC];
			}
			set
			{
				this[KEY_NAME_TIME_SEC] = value;
			}
		}

		// 時間で指定する：ミリ秒
		private const String KEY_NAME_TIME_MILLI_SEC = "TimeMilliSec";
		[UserScopedSetting]
		public String TimeMilliSec
		{
			get
			{
				return (String)this[KEY_NAME_TIME_MILLI_SEC];
			}
			set
			{
				this[KEY_NAME_TIME_MILLI_SEC] = value;
			}
		}

		// 小節で指定する：テンポ
		private const String KEY_NAME_TEMPO = "Tempo";
		[UserScopedSetting]
		[DefaultSettingValue("120")]
		public String Tempo
		{
			get
			{
				return (String)this[KEY_NAME_TEMPO];
			}
			set
			{
				this[KEY_NAME_TEMPO] = value;
			}
		}

		// 小節で指定する：拍数
		private const String KEY_NAME_BEAT = "Beat";
		[UserScopedSetting]
		[DefaultSettingValue("4")]
		public String Beat
		{
			get
			{
				return (String)this[KEY_NAME_BEAT];
			}
			set
			{
				this[KEY_NAME_BEAT] = value;
			}
		}

		// 小節で指定する：小節数
		private const String KEY_NAME_NUM_BARS = "NumBars";
		[UserScopedSetting]
		[DefaultSettingValue("2")]
		public String NumBars
		{
			get
			{
				return (String)this[KEY_NAME_NUM_BARS];
			}
			set
			{
				this[KEY_NAME_NUM_BARS] = value;
			}
		}

		// --------------------------------------------------------------------
		// 終了時の状態
		// --------------------------------------------------------------------

		// 前回起動時のバージョン
		private const String KEY_NAME_PREV_LAUNCH_VER = "PrevLaunchVer";
		[UserScopedSetting]
		[DefaultSettingValue("")]
		public String PrevLaunchVer
		{
			get
			{
				return (String)this[KEY_NAME_PREV_LAUNCH_VER];
			}
			set
			{
				this[KEY_NAME_PREV_LAUNCH_VER] = value;
			}
		}

		// ====================================================================
		// public メンバー関数
		// ====================================================================

		// ====================================================================
		// private 定数
		// ====================================================================

	}
	// public class AutoConcatPartsSettings ___END___

}
// namespace AutoConcatParts.Shared ___END___


