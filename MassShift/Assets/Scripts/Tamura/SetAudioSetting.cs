using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetAudioSetting : MonoBehaviour {
    // オーディオミキサー
    public AudioMixer mixer;

    public void SetAudio() {
        // オーディオミキサー内のグループを指定して再生停止(pitchを0.0fに)
        mixer.SetFloat("gameSEPitch", Time.timeScale);
    }

    public void SetMasterSetting(float _sliderValue) {
        // スライダーの値をデシベルでの倍率に沿って変換
        float volume = Mathf.Clamp(_sliderValue, 0.0001f, 1.0f);    // volumeを0.0001(-80dB)～1.0(0dB)の間の値にして返す
        float volumeDB = 20 * Mathf.Log10(volume);                  // log10(0.0001) = -4, log10(1) = 0　に20を掛けてdB(音量)に変換する
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("masterVolume", Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }

    public void SetBGMSetting(float _sliderValue) {
        // スライダーの値をデシベルでの倍率に沿って変換
        float volume = Mathf.Clamp(_sliderValue, 0.0001f, 1.0f);
        float volumeDB = 20 * Mathf.Log10(volume);
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("bgmVolume", Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }

    public void SetSESetting(float _sliderValue) {
        // スライダーの値をデシベルでの倍率に沿って変換
        float volume = Mathf.Clamp(_sliderValue, 0.0001f, 1.0f);
        float volumeDB = 20 * Mathf.Log10(volume);
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("seVolume", Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }
}
