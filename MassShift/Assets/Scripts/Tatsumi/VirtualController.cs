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
				virtualCtrl = new VirtualController();
			}
			return virtualCtrl;
		}
		set {
			virtualCtrl = value;
		}
	}
	
	[SerializeField]
	float[] virtualAxis = new float[(int)CtrlCode.Max];
	static public float[] VirtualAxis {
		get {
			return VirtualCtrl.virtualAxis;
		}
		private set {
			VirtualCtrl.virtualAxis = value;
		}
	}

	[SerializeField]
	float[] retAxis = new float[(int)CtrlCode.Max];	// 最後にGetAxis()で返した値の配列
	[SerializeField]
	bool selfUpdateRetAxis = true;					// 自身のUpdate()でretAxisを更新するフラグ、falseであれば外部からの呼び出し時のみに更新される

	void Awake() {
		if (VirtualCtrl) {
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
		if (virtualAxis[(int)_ctrl] == 0.0f) {
			retAxis[(int)_ctrl] = Input.GetAxis(_ctrl.ToString());
		} else {
			retAxis[(int)_ctrl] = virtualAxis[(int)_ctrl];
		}
		return retAxis[(int)_ctrl];
	}
}
