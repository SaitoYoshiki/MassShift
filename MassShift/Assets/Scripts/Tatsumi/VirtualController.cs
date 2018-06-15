using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualController : MonoBehaviour {
	public enum CtrlCode {
		Horizontal,
		Vertical,
		Jump,
		Lift,
		Max,
	}

	static VirtualController virtualCtrl = null;
	static VirtualController VirtualCtrl {
		get {
			if (!virtualCtrl) {
				GameObject ctrlObj = new GameObject();
				virtualCtrl = ctrlObj.AddComponent<VirtualController>();
			}
			return virtualCtrl;
		}
		set {
			virtualCtrl = value;
		}
	}

	[SerializeField]
	float[] virtualAxis = new float[(int)CtrlCode.Max];
	float[] VirtualAxis {
		get {
			return VirtualCtrl.virtualAxis;
		}
		set {
			VirtualCtrl.virtualAxis = value;
		}
	}

	[SerializeField]
	float[] virtualAxisHoldTime = new float[(int)CtrlCode.Max];

	[SerializeField]
	bool[] onryVirtualAxis = new bool[(int)CtrlCode.Max];

	[SerializeField]
	float[] retAxis = new float[(int)CtrlCode.Max]; // 最後にGetAxis()で返した値の配列
	[SerializeField]
	bool selfUpdateRetAxis = true;                  // 自身のUpdate()でretAxisを更新するフラグ、falseであれば外部からの呼び出し時のみに更新される

	[SerializeField]
	float defaultHoldTime = 0.5f;

	void Awake() {
		if (virtualCtrl) {
			Debug.LogError("複数のVirtualControllerが生成されました。\n" +
				VirtualCtrl.name + ", " + name);
			enabled = false;
			return;
		} else {
			virtualCtrl = this;
		}
	}

	void Update() {
		if (!selfUpdateRetAxis) return;

		for (CtrlCode code = 0; code < CtrlCode.Max; code++) {
			GetAxis(code);
		}
	}

	public static float GetAxis(CtrlCode _ctrl) {
		return VirtualCtrl.GetControl(_ctrl);
	}

	float GetControl(CtrlCode _ctrl) {
		if (virtualAxisHoldTime[(int)_ctrl] < Time.time) {
			retAxis[(int)_ctrl] = Input.GetAxis(_ctrl.ToString());
		} else {
			retAxis[(int)_ctrl] = virtualAxis[(int)_ctrl];
		}
		return retAxis[(int)_ctrl];
	}

	public static void SetAxis(CtrlCode _ctrl, float _value = 1.0f, float _time = 0.0f) {
		if (_time <= 0.0f) {
			_time = VirtualCtrl.defaultHoldTime;
		}

		VirtualCtrl.VirtualAxis[(int)_ctrl] = _value;
		VirtualCtrl.virtualAxisHoldTime[(int)_ctrl] = (Time.time + _time);
	}
}
