// ============================================================================
// 
// 設定を行うウィンドウ
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using AutoConcatParts.Shared;
using ScriptPortal.Vegas;
using Shinta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AutoConcatParts
{
	public partial class FormSettings : Form
	{
		// ====================================================================
		// コンストラクター・デストラクター
		// ====================================================================

		// --------------------------------------------------------------------
		// コンストラクター
		// --------------------------------------------------------------------
		public FormSettings(List<Media> oVideoMedias, Double oAudioLen)
		{
			InitializeComponent();

			// 初期化
			mVideoMedias = oVideoMedias;
			mAudioLen = oAudioLen;
		}

		// ====================================================================
		// public プロパティー
		// ====================================================================

		// 先頭除外［ミリ秒］
		public Double Head { get; set; }

		// 末尾除外［ミリ秒］
		public Double Tail { get; set; }

		// 間隔［ミリ秒］
		public Double Interval { get; set; }

		// ====================================================================
		// private メンバー変数
		// ====================================================================

		// ビデオメディア群
		private List<Media> mVideoMedias;

		// 音声の長さ
		private Double mAudioLen;

		// 環境設定
		private AutoConcatPartsSettings mAutoConcatPartsSettings;

		// ====================================================================
		// private メンバー関数
		// ====================================================================

		// --------------------------------------------------------------------
		// ミリ秒を可読的な文字列に変換
		// --------------------------------------------------------------------
		private String MillisecondsToTimeString(Double oMilliSec)
		{
			Int32 aMinPart = (Int32)Math.Floor(oMilliSec / (60 * 1000));
			Int32 aSecPart = (Int32)Math.Floor((oMilliSec - aMinPart * 60 * 1000) / 1000);
			Int32 aMilliPart = (Int32)(oMilliSec) % 1000;

			StringBuilder aSB = new StringBuilder();
			if (aMinPart > 0)
			{
				aSB.Append(aMinPart.ToString() + " 分 ");
			}
			aSB.Append(aSecPart.ToString() + " 秒 ");
			aSB.Append(aMilliPart.ToString("D3") + " ミリ秒");
			return aSB.ToString();
		}

		// --------------------------------------------------------------------
		// イベントハンドラー
		// --------------------------------------------------------------------
		private void RadioButtonBy_CheckedChanged(Object oSender, EventArgs oEventArgs)
		{
			// 時間で指定
			TextBoxTimeSec.Enabled = RadioButtonByTime.Checked;
			TextBoxTimeMilliSec.Enabled = RadioButtonByTime.Checked;

			// 小節で指定
			TextBoxTempo.Enabled = RadioButtonByBeat.Checked;
			TextBoxBeat.Enabled = RadioButtonByBeat.Checked;
			TextBoxNumBars.Enabled = RadioButtonByBeat.Checked;
		}

		// --------------------------------------------------------------------
		// イベントハンドラー
		// --------------------------------------------------------------------
		private void TextBoxByBeat_TextChanged(Object oSender, EventArgs oEventArgs)
		{
			LabelByBeat.Text = "（" + MillisecondsToTimeString(TextBoxToInterval()) + "）";
		}

		// --------------------------------------------------------------------
		// テキストボックスに入力されている値から間隔を得る
		// --------------------------------------------------------------------
		private Double TextBoxToInterval()
		{
			return 60.0 * 1000 * Common.StringToInt32(TextBoxBeat.Text) * Common.StringToInt32(TextBoxNumBars.Text) / Common.StringToInt32(TextBoxTempo.Text);
		}

		// --------------------------------------------------------------------
		// テキストボックスに入力されている値からミリ秒を得る
		// ＜引数＞ oTextBoxMin のみ null 可
		// --------------------------------------------------------------------
		private Double TextBoxToMilliSeconds(TextBox oTextBoxMin, TextBox oTextBoxSec, TextBox oTextBoxMilliSec)
		{
			Int32 aMin = 0;
			if (oTextBoxMin != null)
			{
				aMin = Common.StringToInt32(oTextBoxMin.Text);
			}
			Int32 aSec = Common.StringToInt32(oTextBoxSec.Text);
			Int32 aMilliSec = Common.StringToInt32(oTextBoxMilliSec.Text);

			return (Double)aMin * 60 * 1000 + aSec * 1000 + aMilliSec;
		}

		// ====================================================================
		// IDE 生成イベントハンドラー
		// ====================================================================

		private void FormSettings_Shown(object sender, EventArgs e)
		{
			try
			{
				// 環境設定
				mAutoConcatPartsSettings = new AutoConcatPartsSettings();
				VariableSettingsProvider aProvider = (VariableSettingsProvider)mAutoConcatPartsSettings.Providers[VariableSettingsProvider.PROVIDER_NAME_VARIABLE_SETTINGS];
				aProvider.FileName = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Application.UserAppDataPath)))
						+ "\\" + Common.FOLDER_NAME_SHINTA + AcpCommon.APP_ID + "\\" + "user" + Common.FILE_EXT_CONFIG;
				Directory.CreateDirectory(Path.GetDirectoryName(aProvider.FileName));
				mAutoConcatPartsSettings.Reload();

				// 情報
				LabelNumMaterials.Text = mVideoMedias.Count.ToString();
				LabelAudioLen.Text = MillisecondsToTimeString(mAudioLen);

				// 先頭除外
				TextBoxHeadMin.Text = mAutoConcatPartsSettings.HeadMin;
				TextBoxHeadSec.Text = mAutoConcatPartsSettings.HeadSec;
				TextBoxHeadMilliSec.Text = mAutoConcatPartsSettings.HeadMilliSec;

				// 末尾除外
				TextBoxTailMin.Text = mAutoConcatPartsSettings.TailMin;
				TextBoxTailSec.Text = mAutoConcatPartsSettings.TailSec;
				TextBoxTailMilliSec.Text = mAutoConcatPartsSettings.TailMilliSec;

				// 時間で指定
				RadioButtonByTime.Checked = mAutoConcatPartsSettings.ByTime;
				TextBoxTimeSec.Text = mAutoConcatPartsSettings.TimeSec;
				TextBoxTimeMilliSec.Text = mAutoConcatPartsSettings.TimeMilliSec;

				// 小節で指定
				RadioButtonByBeat.Checked = !mAutoConcatPartsSettings.ByTime;
				TextBoxTempo.Text = mAutoConcatPartsSettings.Tempo;
				TextBoxBeat.Text = mAutoConcatPartsSettings.Beat;
				TextBoxNumBars.Text = mAutoConcatPartsSettings.NumBars;

				// アプリ情報
				LabelAppInfo.Text = AcpCommon.APP_VER + "\n" + AcpCommon.COPYRIGHT_J;

			}
			catch (Exception oExcep)
			{
				MessageBox.Show("設定ウィンドウ表示時エラー：\n" + oExcep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ButtonOK_Click(object sender, EventArgs e)
		{
			try
			{
				// プロパティー設定
				Head = TextBoxToMilliSeconds(TextBoxHeadMin, TextBoxHeadSec, TextBoxHeadMilliSec);
				if (Head < 0.0)
				{
					throw new Exception("先頭の使用しない時間は 0 ミリ秒以上の値を指定して下さい。");
				}
				Tail = TextBoxToMilliSeconds(TextBoxTailMin, TextBoxTailSec, TextBoxTailMilliSec);
				if (Tail < 0.0)
				{
					throw new Exception("末尾の使用しない時間は 0 ミリ秒以上の値を指定して下さい。");
				}
				if (RadioButtonByTime.Checked)
				{
					Interval = TextBoxToMilliSeconds(null, TextBoxTimeSec, TextBoxTimeMilliSec);
				}
				else
				{
					Interval = TextBoxToInterval();
				}
				if (Interval < 500.0)
				{
					throw new Exception("切り貼りの間隔は 500 ミリ秒以上の値を指定して下さい。");
				}

				//// 状態保存

				// 先頭除外
				mAutoConcatPartsSettings.HeadMin = TextBoxHeadMin.Text;
				mAutoConcatPartsSettings.HeadSec = TextBoxHeadSec.Text;
				mAutoConcatPartsSettings.HeadMilliSec = TextBoxHeadMilliSec.Text;

				// 末尾除外
				mAutoConcatPartsSettings.TailMin = TextBoxTailMin.Text;
				mAutoConcatPartsSettings.TailSec = TextBoxTailSec.Text;
				mAutoConcatPartsSettings.TailMilliSec = TextBoxTailMilliSec.Text;

				// 時間で指定
				mAutoConcatPartsSettings.ByTime = RadioButtonByTime.Checked;
				mAutoConcatPartsSettings.TimeSec = TextBoxTimeSec.Text;
				mAutoConcatPartsSettings.TimeMilliSec = TextBoxTimeMilliSec.Text;

				// 小節で指定
				mAutoConcatPartsSettings.Tempo = TextBoxTempo.Text;
				mAutoConcatPartsSettings.Beat = TextBoxBeat.Text;
				mAutoConcatPartsSettings.NumBars = TextBoxNumBars.Text;

				mAutoConcatPartsSettings.PrevLaunchVer = AcpCommon.APP_VER;
				mAutoConcatPartsSettings.Save();

				DialogResult = DialogResult.OK;
			}
			catch (Exception oExcep)
			{
				MessageBox.Show("OK ボタンクリック時エラー：\n" + oExcep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
	// public partial class FormSettings ___END___

}
// namespace AutoConcatParts ___END___