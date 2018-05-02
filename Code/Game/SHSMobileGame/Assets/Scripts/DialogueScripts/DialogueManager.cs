using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	

using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour {
	//keep track of all sentences in a dialogue, we load all sentences from the end of queue

	public Terminal terminal;

	public Text nameText;
	public Text dialogueText;

	public Queue<string> sentences;

	bool inside = true;

	Button menuButton;
	Button achievementButton;
	Button leaderBoardButton;
	Color defaultColour;
	Color highlightColor = Color.red;
	//int n = 2;
	static int listN = 2;
	static int textN = 4;
	Button []list = new Button[listN];
	Text[] text = new Text[textN];

	Color defaultButtonColor ;

	GameObject[] safeZones;

	int slide = 0;
	int counterCurrent = 0;
	int counterPrev;

	int textCounter = 0;
	int textCounterPrev;

	Color[] colorsButton = new Color[listN];
	Color[] colorsText = new Color[textN];

	Color[] colorsSafeModes;

	GameObject player;
	GameObject mainCamera;
	GameObject turretButton;

	public enum UPDATEMODE {ROLEX, PLAYER_IN, OUT, IN_SAFEMODE, IN, FLAG, DEFAULT, MED, TURRET};

	private UPDATEMODE updateMode = UPDATEMODE.DEFAULT;


	Color switchButtonColor;
	Button switchButton;

	Button actionButton;
	Button improveButton;


	float deltaX = 0.1f;


	GameObject continueButton;
	GameObject continueButton2;


	// Use this for initialization
	void Start () {

		continueButton = GameObject.Find ("continue");
		continueButton2 = GameObject.Find ("ClickAlsoWorking");
		continueButton.SetActive (false);
		continueButton2.SetActive (false);

		actionButton = GameObject.Find ("ActionButton").GetComponent<Button>();
		improveButton = GameObject.Find ("ImproveButton").GetComponent<Button> ();

		switchButton = GameObject.Find ("ModeButton").GetComponent<Button>();
		switchButtonColor = switchButton.GetComponent<Image> ().color;

		defaultButtonColor = switchButtonColor;


		sentences = new Queue<string>();

		turretButton = GameObject.Find ("Turret_Button");


		turretButton.SetActive (false);

		menuButton = GameObject.Find ("MenuButton").GetComponent<Button> ();
		achievementButton = GameObject.Find ("AchievementButton").GetComponent<Button> ();
		leaderBoardButton = GameObject.Find ("LeaderboardButton").GetComponent<Button> ();

		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");

		colorsSafeModes = new Color[safeZones.Length];

		for (int i = 0; i < colorsSafeModes.Length; i++) {
			colorsSafeModes [i] = safeZones [i].GetComponent<Renderer> ().material.color;
		}


		player = GameObject.Find ("PlayerNotSafeZone");
		mainCamera = GameObject.FindWithTag ("MainCamera");

		list [0] = achievementButton;	
		list [1] = leaderBoardButton;

		text[0] = GameObject.Find ("TeamText").GetComponent<Text> ();
		text[1] = GameObject.Find ("CreditText").GetComponent<Text> ();
		text[2] = GameObject.Find ("MultiplierText").GetComponent<Text> ();
		text[3] = GameObject.Find ("LevelText").GetComponent<Text> ();


		for (int i = 0; i < colorsButton.Length; i++) {
			colorsButton [i] = list [i].GetComponent<Image> ().color;
		}

		for (int i = 0; i < colorsText.Length; i++) {
			colorsText [i] = text [i].color;
		}

		defaultColour = menuButton.GetComponent<Image>().color;
		textCounterPrev = textCounter;

		terminal = new Terminal ();



	}

	public void skipButton(){

		SceneManager.LoadScene("MainScene");

	}




	public void StartDialogue(Dialogue dialogue){
		//Debug.Log ("Starting conversation with " + dialogue.name);

		GameObject startButton = GameObject.FindWithTag ("StartButton");
		GameObject skipButton = GameObject.Find ("SkipTutorialButton");

		continueButton.SetActive (true);
		continueButton2.SetActive (true);


		skipButton.SetActive (false);
		startButton.SetActive (false);

		sentences.Clear ();
		foreach (string sentence in dialogue.sentences) {
			sentences.Enqueue (sentence);
		}
		DisplayNextSentence ();
	}

	private void ResetColors(){

		for (int i = 0; i < colorsSafeModes.Length; i++) {
			this.safeZones [i].GetComponent<Renderer> ().material.color = colorsSafeModes [i];
		}

		switchButton.GetComponent<Image> ().color = switchButtonColor;

		actionButton.GetComponent<Image> ().color = switchButtonColor;
		improveButton.GetComponent<Image> ().color = switchButtonColor;

	}

	public void DisplayNextSentence(){

		if (sentences.Count == 0) {
			StopAllCoroutines ();
			EndDialogue();




			return;
		}

		//we go through the button sliders 
		string sentence = sentences.Dequeue ();

		StopAllCoroutines ();


		ResetColors ();
		Debug.Log ("slide " + slide);
	
		StartCoroutine (TypeSentence (sentence));

		switch (slide) {
		case 4:
			triggerZoomOutForSafeZones ();	
			StartCoroutine (BlinkAllZones ());
			break;

		case 5:
			updateMode = UPDATEMODE.IN;
			StartCoroutine (BlinkButton(switchButton));
			break;

		case 6: 
			updateMode = UPDATEMODE.PLAYER_IN;
			break;

		case 7: 
			updateMode = UPDATEMODE.MED;
			break;

		case 8:

			turretButton.SetActive (true);

			Button turret = GameObject.Find ("Turret_Button").GetComponent<Button> ();

			StartCoroutine (BlinkButton (turret));
			break;

		case 9:

			turretButton.SetActive (false);

			Vector3 positions = new Vector3 (-2.13f, 4.66f, -8.11f);
			Vector3 rotation = new Vector3 (-0.308f, -258.860f, 1.374f);

			GameObject gameObject = Instantiate (Resources.Load ("Terminal"), positions, Quaternion.Euler (rotation)) as GameObject;
			gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
			gameObject.transform.position = positions;

			Vector3 positionLaser = new Vector3 (-3f, 6.8f, -8f);
			Instantiate (
				Resources.Load ("Effects/Prefabs/" + "Blue" + "LaserEffect"),
				positionLaser,
				Quaternion.Euler (new Vector3 (19.797f, 86.346f, -1.239f)));

			updateMode = UPDATEMODE.TURRET;


			Text actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
			actionButtonText.text = "Buff ✓";


			Text improveButtonText = improveButton.transform.Find ("Text").GetComponent<Text> ();
			improveButtonText.text = "Improve ✓";

			StartCoroutine (BlinkButton (actionButton));
			StartCoroutine (BlinkButton (improveButton));

			//actionButtonText.text = "Buff "+ (-QuantitiesConstants.TERMINAL_BUFF_COST) +"$";
			//improveButtonText.text = "Improve "+ QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [terminalLevel + 1] +"₡";
			break;


		case 10:

			actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
			actionButtonText.text = "Smash";

			improveButtonText = improveButton.transform.Find ("Text").GetComponent<Text> ();
			improveButtonText.text = "No Action";

			StartCoroutine (BlinkButton (actionButton));
			break;


		case 12:

			actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
			actionButtonText.text = "Heal";

			improveButtonText = improveButton.transform.Find ("Text").GetComponent<Text> ();
			improveButtonText.text = "Improve";

			StartCoroutine (BlinkButton (actionButton));
			StartCoroutine (BlinkButton (improveButton));

			break;

		case 14:
			
			continueButton2.SetActive (false);
			Button button = continueButton.GetComponent<Button> ();

			button.transform.Find ("Text").GetComponent<Text> ().text = "Start Game";



			break;
		}



	}

	private void triggerZoomOutForSafeZones(){
		Debug.Log ("hello");
		updateMode = UPDATEMODE.OUT;
	}


	public IEnumerator BlinkAllZones(){

		Debug.Log ("blinkAllZonees");

		colorsSafeModes = new Color[this.safeZones.Length];

		for(int i = 0 ; i < colorsSafeModes.Length ; i++){
			colorsSafeModes[i] = safeZones [i].GetComponent<Renderer> ().material.color;
		}

		while(true){

			for (int i = 0; i < safeZones.Length; i++) {
				safeZones [i].GetComponent<Renderer> ().material.color = highlightColor;
			}

			yield return new WaitForSeconds(.5f);

			for (int i = 0; i < safeZones.Length; i++) {
				safeZones [i].GetComponent<Renderer> ().material.color = colorsSafeModes[i];
			}

			yield return new WaitForSeconds(.5f);
		}

	}

	public IEnumerator BlinkSwitchButton(){

		while (true) {

			switchButton.GetComponent<Image> ().color = highlightColor;
			yield return new WaitForSeconds(.5f);

			switchButton.GetComponent<Image> ().color = switchButtonColor;
			yield return new WaitForSeconds(.5f);
		}

	}


	public IEnumerator BlinkButton(Button button){
		while (true) {

			button.GetComponent<Image> ().color = highlightColor;
			yield return new WaitForSeconds (.5f);

			button.GetComponent<Image> ().color = defaultButtonColor;
			yield return new WaitForSeconds (.5f);


		}

	}


	void Update(){
		//TODO 

		switch (updateMode) {
		//ZOOM CAMERA OUT FOR SAFE ZONES
		case UPDATEMODE.OUT:
			float i = mainCamera.transform.position.y;

			if (i < 250) {

				Vector3 newPos = mainCamera.transform.position;
				i = i + 2;

				Vector3 n = new Vector3 (newPos.x, i, newPos.z);
				mainCamera.transform.position = n;
			}

			break;

		

	    
		//ZOOM CAMERA IN
		case UPDATEMODE.IN:

			float x = mainCamera.transform.position.x;
			float y = mainCamera.transform.position.y;

			//Inside a safe zone movement
			/*if (x < 19) {

				Vector3 newPos = mainCamera.transform.position;
				x = x + 1;

				Vector3 n = new Vector3 (x, newPos.y, newPos.z);
				mainCamera.transform.position = n;

			}else{
*/

			if (y > 70) {

				Vector3 newPos = mainCamera.transform.position;
				y = y - 2;

				Vector3 n = new Vector3 (newPos.x, y, newPos.z);
				mainCamera.transform.position = n;
			} else {

				if (x < 19) {

					Vector3 newPos = mainCamera.transform.position;
					x = x + 2;

					Vector3 n = new Vector3 (x, newPos.y, newPos.z);
					mainCamera.transform.position = n;

				}

			}

			//}

			break;

			//MOVE TO SAFE ZONE
		case UPDATEMODE.PLAYER_IN:
			//player movement
			i = -player.transform.position.x;

			if (i > -25.85) {

				Vector3 newPos = player.transform.position;
				i = i - 0.5f;

				Vector3 n = new Vector3 (-i, newPos.y, newPos.z);
				player.transform.position = n;

			} else {
				//player.GetComponent<Material> ().color = Color.green;
			}
			break;

			//Move to MED
		case UPDATEMODE.MED:
			x = -player.transform.position.x;

			if (x < -4.4) {

				Vector3 newPos = player.transform.position;
				x = x + 0.5f;

				Vector3 n = new Vector3 (-x, newPos.y, newPos.z);
				player.transform.position = n;

			} else {

				float z = -player.transform.position.z;

				if (z > 8.3) {

					Vector3 newPos = player.transform.position;
					z = z - 0.6f;

					Vector3 n = new Vector3 (newPos.x, newPos.y, -z);
					player.transform.position = n;
				} else {

					//Replace camera 

					x = mainCamera.transform.position.x;
					z = mainCamera.transform.position.z;

					//Inside a safe zone movement
					if (x > 0.0f) {

						Vector3 newPos = mainCamera.transform.position;
						x = x - 2;

						Vector3 n = new Vector3 (x, newPos.y, newPos.z);
						mainCamera.transform.position = n;

					}

					if (z < -15.0f) {

						Vector3 newPos = mainCamera.transform.position;
						z = z + 1;

						Vector3 n = new Vector3 (newPos.x, newPos.y, z);
						mainCamera.transform.position = n;
					}
				}
			}

			break;

		case UPDATEMODE.TURRET:

			x = mainCamera.transform.position.x;
			y = mainCamera.transform.position.y;
			float z_ = mainCamera.transform.position.z;

			float rotate_x = mainCamera.transform.rotation.eulerAngles.x;

			//Inside a safe zone movement
			if (x > 0.0f) {

				Vector3 newPos = mainCamera.transform.position;
				x = x - 1;

				Vector3 n = new Vector3 (x, newPos.y, newPos.z);
				mainCamera.transform.position = n;

			}

			if(y > 39.0f){

				Vector3 newPos = mainCamera.transform.position;
				y = y - 1;

				Vector3 n = new Vector3 (newPos.x, y, newPos.z);
				mainCamera.transform.position = n;

			}

			if (z_ > -25.0f) {

				Vector3 newPos = mainCamera.transform.position;
				z_ = z_ - 1;

				Vector3 n = new Vector3 (newPos.x, newPos.y, z_);
				mainCamera.transform.position = n;
			}

			//Debug.Log (rotate_x);

			if (rotate_x > 59.0f) {

				Vector3 newRot = mainCamera.transform.rotation.eulerAngles;
				rotate_x = rotate_x - 1;

				Vector3 n = new Vector3 (rotate_x, newRot.y, newRot.z);
				mainCamera.transform.rotation = Quaternion.Euler(n);

			}

			break;
		}




	}


	public IEnumerator BlinkText(Text txt,Color prev){
		//blink it forever. You can set a terminating condition depending upon your requirement

		text [textCounterPrev].color = prev;

		textCounterPrev = textCounter;
		textCounter++;

		Color color = txt.color;
		defaultColour = color;

		while(true){

			txt.color = defaultColour;
			yield return new WaitForSeconds(.5f);

			txt.color = highlightColor;
			yield return new WaitForSeconds(.5f);
		}
	}

	public IEnumerator BlinkButton(Button menuButton,Color prev){
		//blink it forever. You can set a terminating condition depending upon your requirement

		list [counterPrev].GetComponent<Image> ().color = prev;

		counterPrev = counterCurrent;
		counterCurrent++;

		Color color = menuButton.GetComponent<Image> ().color;
		defaultColour = color;

		while(true){

			menuButton.GetComponent<Image> ().color = defaultColour;
			yield return new WaitForSeconds(.5f);

			menuButton.GetComponent<Image> ().color = highlightColor;
			yield return new WaitForSeconds(.5f);
		}
	}

	IEnumerator TypeSentence(string sentence){

		continueButton2.SetActive (false);
		continueButton.SetActive (false);

		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray()) {

			dialogueText.text += letter;
			yield return null;
		}

		slide++;

		continueButton.SetActive (true);
		continueButton2.SetActive (true);

	}

	void EndDialogue(){
		SceneManager.LoadScene ("InitialScene");
	}


}
