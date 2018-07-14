using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//メイン
public class Main : MonoBehaviour {
	//参照
	public GameObject timer;   //タイマー
	public GameObject counter; //カウンタ
	public GameObject goal;    //ゴール
	public GameObject car;     //車
	public GameObject pointer; //視線ポインタ

	//状態
	enum State { Wait, Ready, Play, Goal };
	State state; //状態

	//情報
	float time;            //時間
	float count;           //カウント
	float goalWaitTime;    //ゴール待機時間
	float pointerWaitTime; //視線ポインタ待機時間

	//チェックポイント
	const int CHECKPOINT_NUM = 3; //チェックポイント数
	int checkPointCount;          //チェックポイント通過数
	int checkPointLastNo;         //チェックポイント最終No

	//スタート時に呼ばれる
	void Start() {
		//Xボタン押下でアプリ終了
		Input.backButtonLeavesApp = true;

		//待機への遷移
		SetState (State.Wait);
	}

	//状態の指定
	void SetState (State state) {
		this.state = state;

		//待機
		if (this.state == State.Wait) {
			//情報の初期化
			this.time = 0f;
			this.count = 3f;
			this.goalWaitTime = 0f;
			this.pointerWaitTime = -1;

			//チェックポイントの初期化
			this.checkPointCount = 0;
			this.checkPointLastNo = 0;

			//参照の初期化
			this.timer.GetComponent<Text>().text = "カウンタ見つめてスタート";
			this.counter.GetComponent<Text>().text = "" + (int)this.count;
			this.goal.GetComponent<Text>().text = "";
			InitCar ();
			//this.pointer.SetActive (true);
		}
		//準備
		else if (this.state == State.Ready) {
			this.timer.GetComponent<Text>().text = "0'00\"00";
			//this.pointer.SetActive (false);
		}
		//プレイ
		else if (this.state == State.Play) {
			this.counter.GetComponent<Text>().text = "";
			this.car.GetComponent <Car> ().enabled = true;
		}
		//ゴール
		else if (this.state == State.Goal) {
			this.goal.GetComponent<Text>().text = "GOAL";
			this.goalWaitTime = 0f;
		}
	}

	//フレーム毎に呼ばれる
	void Update () {
		//待機
		if (this.state == State.Wait) {
			//視線ポインタによるクリック
			if (IsPointerClick ()) {
				//Readyに遷移
				SetState (State.Ready);
			}

			//マウスによるクリック
			if (Input.GetMouseButton(0)) {
				//Readyに遷移
				SetState (State.Ready);
			}
		}
		//準備
		else if (this.state == State.Ready) {
			//カウンタの更新
			UpdateCounter ();

			//レース開始
			if (this.count < 1f) {
				SetState (Main.State.Play);
			}
		}
		//プレイ
		else if (this.state == State.Play) {
			//カウンタの更新
			UpdateCounter ();

			//タイマーの更新
			UpdateTimer ();
		}
		//ゴール
		else if (this.state == State.Goal) {
			//3秒後
			this.goalWaitTime += Time.deltaTime;
			if (this.goalWaitTime > 3f) {
				//待機に遷移
				SetState (State.Wait);
			}
		}
	}

	//車の初期化
	void InitCar () {
		this.car.SetActive (false);
		this.car.SetActive (true);
		this.car.GetComponent <Car> ().enabled = false;
		this.car.transform.position = new Vector3(-40f, 0f, 0f);
		this.car.transform.rotation = Quaternion.Euler(0, 0, 0);
		this.car.GetComponent<Rigidbody> ().velocity = new Vector3(0, 0, 0);
		this.car.GetComponent<Rigidbody> ().angularVelocity = new Vector3(0, 0, 0);
	}

	//カウンタの更新
	void UpdateCounter () {
		string text = "";
		if (this.count >= 0f) {
			this.count -= Time.deltaTime;
			text = "" + (int)this.count;
		}
		if (this.counter.GetComponent<Text> ().text != text) {
			this.counter.GetComponent<Text> ().text = text;
		}
	}

	//タイマーの更新
	void UpdateTimer () {
		this.time += Time.deltaTime;
		int minute = (int)(this.time / 60);
		int second = (int)(this.time % 60);
		int msecond = (int)(this.time * 100 % 60);
		string text =
			minute.ToString ("0") + "'" +
			second.ToString ("00") + "\"" +
			msecond.ToString ("00");
		if (this.timer.GetComponent<Text>().text != text) {
			this.timer.GetComponent<Text>().text = text;
		}
	}

	//車がチェックポイントに入った時に呼ばれる
	public void OnCheckPointEnter(int checkPointNo) {
		if (this.state == Main.State.Play) {
			//チェックポイントの位置の遷移
			if ((this.checkPointLastNo + 1) % CHECKPOINT_NUM == checkPointNo) {
				this.checkPointCount += 1;
			}
			if ((this.checkPointLastNo - 1 + CHECKPOINT_NUM) % CHECKPOINT_NUM == checkPointNo) {
				this.checkPointCount -= 1;
			}
			this.checkPointLastNo = checkPointNo;

			//Goalに遷移
			if (this.checkPointCount == CHECKPOINT_NUM) {
				SetState (State.Goal);
			}
		}
	}

	//視線ポインタがオブジェクトを1秒以上選択しているかどうかの取得
	bool IsPointerClick() {
		if (this.pointerWaitTime >= 0f) {
			this.pointerWaitTime += Time.deltaTime;
			return (this.pointerWaitTime >= 1f);
		}
		return false;
	}

	//視線ポインタがオブジェクトを選択した時に呼ばれる
	public void OnEnterPointer(){
		this.pointerWaitTime = 0;
	}

	//視線ポインタがオブジェクトを選択解除した時に呼ばれる
	public void OnExitPointer(){
		this.pointerWaitTime = -1;
	}
}
