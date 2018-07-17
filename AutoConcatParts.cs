// ============================================================================
//
// Vegas 用動画切り貼りスクリプト
// （音声トラックの長さに合わせて、動画を切り貼りして作成する）
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using AutoConcatParts;
using AutoConcatParts.Shared;
using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

class EntryPoint
{
	// ====================================================================
	// public メンバー関数
	// ====================================================================

	// --------------------------------------------------------------------
	// エントリーポイント
	// --------------------------------------------------------------------
	public void FromVegas(Vegas oVegas)
	{
		try
		{
			mVegas = oVegas;

			// 確認
			List<Media> aVideoMedias;
			Double aAudioLen;
			Check(out aVideoMedias, out aAudioLen);

			// 設定
			Double aHead;
			Double aTail;
			Double aInterval;
			Settings(aVideoMedias, aAudioLen, out aHead, out aTail, out aInterval);

			// 実行
			List<VideoPart> aVideoParts;
			GenerateParts(aVideoMedias, aHead, aTail, aInterval, out aVideoParts);
			ConcatParts(aVideoParts, aAudioLen, aInterval);

			MessageBox.Show("完了しました。", "報告", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		catch (Exception oExcep)
		{
			MessageBox.Show("切り貼り時エラー：\n" + oExcep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	// ====================================================================
	// private メンバー定数
	// ====================================================================

	// パーツを格納する MediaBin の名前
	private const String MEDIA_BIN_NAME = "AutoConcatParts";

	// 動画を配置するトラックの名前
	private const String TRACK_NAME = "AutoConcatVideo";

	// ====================================================================
	// private メンバー変数
	// ====================================================================

	// VEGAS オブジェクト
	private Vegas mVegas;

	// ====================================================================
	// private メンバー関数
	// ====================================================================

	// --------------------------------------------------------------------
	// 動作状況の確認
	// ＜引数＞ oVideoMedias: ビデオを含むメディア群, oAudioLen: 最長の音声の長さ[ミリ秒]
	// ＜例外＞ Exception
	// --------------------------------------------------------------------
	private void Check(out List<Media> oVideoMedias, out Double oAudioLen)
	{
		// 動画が選択されているか
		oVideoMedias = new List<Media>();
		Media[] aMedias = mVegas.Project.MediaPool.GetSelectedMedia();
		foreach (Media aMedia in aMedias)
		{
			VideoStream aVideoStream = aMedia.GetVideoStreamByIndex(0);
			if (aVideoStream != null)
			{
				oVideoMedias.Add(aMedia);
			}
		}
		if (oVideoMedias.Count == 0)
		{
			throw new Exception("プロジェクトメディアの中から素材動画を選択してから、スクリプトを実行して下さい。");
		}

		// 有効な音声トラックがあるか
		oAudioLen = 0.0;
		foreach (Track aTrack in mVegas.Project.Tracks)
		{
			if (aTrack.IsAudio())
			{
				if (aTrack.Length.ToMilliseconds() > oAudioLen)
				{
					oAudioLen = aTrack.Length.ToMilliseconds();
				}
			}
		}
		if (oAudioLen == 0.0)
		{
			throw new Exception("音源をタイムラインに登録してから、スクリプトを実行して下さい。");
		}
	}

	// --------------------------------------------------------------------
	// パーツを連結
	// --------------------------------------------------------------------
	private void ConcatParts(List<VideoPart> oVideoParts, Double oAudioLen, Double oInterval)
	{
		// トラック作成
		VideoTrack aVideoTrack = new VideoTrack(mVegas.Project, 0, TRACK_NAME);
		mVegas.Project.Tracks.Add(aVideoTrack);

		// パーツ使用数
		Int32 aUseNum = (Int32)Math.Ceiling(oAudioLen / oInterval);
		PartsUseMode aPartsUseMode;
		if (oVideoParts.Count >= aUseNum * 3)
		{
			aPartsUseMode = PartsUseMode.OnceGroup;
		}
		else if (oVideoParts.Count >= aUseNum)
		{
			aPartsUseMode = PartsUseMode.Once;
		}
		else
		{
			aPartsUseMode = PartsUseMode.Reuse;
		}

		// パーツ追加
		Random aRandom = new Random();
		for (Int32 i = 0; i < aUseNum; i++)
		{
			// パーツ番号
			Int32 aPartsIndex = aRandom.Next(0, oVideoParts.Count);

			// パーツ
			VideoEvent aVideoEvent = aVideoTrack.AddVideoEvent(new Timecode(i * oInterval), new Timecode(oInterval));
			Take aTake = aVideoEvent.AddTake(oVideoParts[aPartsIndex].VideoStream);
			aTake.Offset = oVideoParts[aPartsIndex].Offset;

			// パーツ廃棄
			switch (aPartsUseMode)
			{
				case PartsUseMode.Once:
					oVideoParts.RemoveAt(aPartsIndex);
					break;
				case PartsUseMode.OnceGroup:
					for (Int32 j = aPartsIndex + 1; j >= aPartsIndex - 1; j--)
					{
						if (0 <= j && j < oVideoParts.Count)
						{
							oVideoParts.RemoveAt(j);
						}
					}
					break;
			}
		}
	}

	// --------------------------------------------------------------------
	// 素材動画からパーツを作成
	// --------------------------------------------------------------------
	private void GenerateParts(List<Media> oVideoMedias, Double oHead, Double oTail, Double oInterval, out List<VideoPart> oVideoParts)
	{
		oVideoParts = new List<VideoPart>();

		foreach (Media aMedia in oVideoMedias)
		{
			VideoStream aVideoStream = aMedia.GetVideoStreamByIndex(0);
			Double aMediaLen = aMedia.Length.ToMilliseconds();
			Double aPos = oHead;
			while (aPos <= aMediaLen - oTail - oInterval)
			{
				VideoPart aVideoPart = new VideoPart();
				aVideoPart.VideoStream = aVideoStream;
				aVideoPart.Offset = new Timecode(aPos);
				oVideoParts.Add(aVideoPart);

				aPos += oInterval;
			}
		}

		if (oVideoParts.Count == 0)
		{
			throw new Exception("素材動画からパーツを作成できませんでした。\n間隔や除外時間が長すぎないか確認して下さい。");
		}
	}


	// --------------------------------------------------------------------
	// 設定ウィンドウの表示
	// ＜例外＞ Exception
	// --------------------------------------------------------------------
	private void Settings(List<Media> oVideoMedias, Double oAudioLen, out Double oHead, out Double oTail, out Double oInterval)
	{
		using (FormSettings aFormSettings = new FormSettings(oVideoMedias, oAudioLen))
		{
			if (aFormSettings.ShowDialog() != DialogResult.OK)
			{
				throw new Exception("中止しました。");
			}

			oHead = aFormSettings.Head;
			oTail = aFormSettings.Tail;
			oInterval = aFormSettings.Interval;
		}
	}


}
// class EntryPoint ___END___


