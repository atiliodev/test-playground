// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {



	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script

	public BikesControlerSystem bikes;

	// Use this for initialization
	void Start () {
		ctrlHub = GameObject.Find("gameScene");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
	}
	
	// Update is called once per frame
	void Update () {
		//////////////////////////////////// ACCELERATE, braking & 'full throttle - manual trick' //////////////////////////////////////////////
		//Alpha2 is key "2". Used to make manual. Also, it can be achived by 100% "throtle on mobile joystick"
		if (!Input.GetKey (KeyCode.Alpha2)) {
			outsideControls.Vertical = Input.GetAxis ("VInput") / 1.112f;//to get less than 0.9 as acceleration to prevent wheelie(wheelie begins at >0.9)
			if(Input.GetAxis ("VInput") <0) outsideControls.Vertical = outsideControls.Vertical * 1.112f;//need to get 1(full power) for front brake
		}

		//////////////////////////////////// STEERING /////////////////////////////////////////////////////////////////////////
		if (bikes.bikeController.crashed)
		{
			outsideControls.Horizontal = Mathf.Lerp(0, -1, 3 * Time.deltaTime);
		}
		else
		{
			outsideControls.Horizontal = Input.GetAxis("HInput");
		}
		if (Input.GetKey (KeyCode.Alpha2)) outsideControls.Vertical = 1;
		//}

		//////////////////////////////////// Rider's mass translate ////////////////////////////////////////////////////////////
		//this strings controls pilot's mass shift along bike(vertical)
		if (Input.GetKey ("up")) {
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift += 0.1f;
			if (outsideControls.VerticalMassShift > 1.0f) outsideControls.VerticalMassShift = 1.0f;
		}

		if (Input.GetKey("down")){
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift -= 0.1f;
			if (outsideControls.VerticalMassShift < -1.0f) outsideControls.VerticalMassShift = -1.0f;
		}
		if(!Input.GetKey("up") && !Input.GetKey("down")) outsideControls.VerticalMassShift = 0;

		//this strings controls pilot's mass shift across bike(horizontal)
		if (Input.GetKey(KeyCode.D)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift += 0.1f;
			if (outsideControls.HorizontalMassShift >1.0f) outsideControls.HorizontalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.A)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift -= 0.1f;
			if (outsideControls.HorizontalMassShift < -1.0f) outsideControls.HorizontalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) outsideControls.HorizontalMassShift = 0;


		//////////////////////////////////// Rear Brake ////////////////////////////////////////////////////////////////
		// Rear Brake
		if (Input.GetKey ("right ctrl")) {
			outsideControls.rearBrakeOn = true;
		} else
			outsideControls.rearBrakeOn = false;

		//////////////////////////////////// Restart ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if (Input.GetKey (KeyCode.R)) {
			outsideControls.restartBike = true;
		} else
			outsideControls.restartBike = false;

		// LeftShift for full restart
		if (Input.GetKey (KeyCode.LeftShift)) {
			outsideControls.fullRestartBike = true;
		} else
			outsideControls.fullRestartBike = false;

		//////////////////////////////////// Reverse ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if(Input.GetKeyDown(KeyCode.C)){
				outsideControls.reverse = true;
		} else outsideControls.reverse = false;
		///
	}
}
