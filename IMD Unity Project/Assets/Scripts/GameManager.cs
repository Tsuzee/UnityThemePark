using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	//prefabs
    GameObject myGuy;
	public GameObject TargetPrefab;
    public GameObject LeaderPrefab;
	public GameObject GuyPrefab;
    public GameObject ScaryPrefab;
	public GameObject ObstaclePrefab;
    public GameObject Center;

    public GameObject player;

    //panel to fade the screen
    private Image screenFader;
    public bool FadeScreen;
    private bool notFaded = true;
    private float fadeSpeed = 2;

	public int numObst;

    private List<GameObject> flock;
    public List<GameObject> Flock {
        get { return flock;}
    }

    public List<GameObject> modFlock;


    private List<GameObject> leaders;
    public List<GameObject> Leaders {
        get { return leaders;}
    }

    private Vector3 flockDirection;
    public Vector3 FlockDirection {
        get { return flockDirection;}
    }
    
    private Vector3 centroid;
    public Vector3 Centroid {
        get { return centroid; }
    }
    
    public int numOfFlockers;
    public int numOfWanderingScary;
    public int numOfLeaders;

	///<summary>
	/// Game Cameras for the player to switch between
	/// The third camera is coded as a follow the guy camera
	/// </summary>
	public Camera[] cameras;
	private int currentCameraIndex;


	/// <summary>
	/// Game lighting
	/// </summary>

	///player flashlight
	public Light flashLight;
	
	/// Main Carousel Light;
	public Light CarouselLight;

	///Hurricane Lights
	public Light HurCarLight1;
	public Light HurCarLight2;
	public Light HurCarLight3;
	public Light HurCarLight4;
	public Light HurCarLight5;
	public Light HurRideLight;

    //TeaCup Light
    public Light TeaCupLight;

    //RoundUp Light
    public Light RoundUpLight;

	/// <summary>
	/// Should lights in the scene be on
	/// </summary>
	public bool lightsOn;

	/// <summary>
	/// The number of real-world seconds in one game day.
	/// </summary>
	public float dayCycleLength;
	
	/// <summary>
	/// The current time within the day cycle. Modify to change the World Time.
	/// </summary>
	public float currentCycleTime;
	
	//Would be the amount of time the sky takes to transition if UpdateSkybox were used.
	//public float skyTransitionTime;
	
	/// <summary>
	/// The current 'phase' of the day; Dawn, Day, Dusk, or Night
	/// </summary>
	public DayPhase currentPhase;
	
	/// <summary>
	/// The number of hours per day used in the WorldHour time calculation.
	/// </summary>
	public float hoursPerDay;
	
	/// <summary>
	/// Dawn occurs at currentCycleTime = 0.0f, so this offsets the WorldHour time to make
	/// dawn occur at a specified hour. A value of 3 results in a 5am dawn for a 24 hour world clock.
	/// </summary>
	public float dawnTimeOffset;
	
	/// <summary>
	/// The calculated hour of the day, based on the hoursPerDay setting. Do not set this value.
	/// Change the time of day by calculating and setting the currentCycleTime.
	/// </summary>
	public int worldTimeHour;
	
	/// <summary>
	/// The scene ambient color used for full daylight.
	/// </summary>
	public Color fullLight;
	
	/// <summary>
	/// The scene ambient color used for full night.
	/// </summary>
	public Color fullDark;
	
	/// <summary>
	/// The scene skybox material to use at dawn and dusk.
	/// </summary>
	public Material dawnDuskSkybox;
	
	/// <summary>
	/// The scene fog color to use at dawn and dusk.
	/// </summary>
	public Color dawnDuskFog;
	
	/// <summary>
	/// The scene skybox material to use during the day.
	/// </summary>
	public Material daySkybox;
	
	/// <summary>
	/// The scene fog color to use during the day.
	/// </summary>
	public Color dayFog;
	
	/// <summary>
	/// The scene skybox material to use at night.
	/// </summary>
	public Material nightSkybox;
	
	/// <summary>
	/// The scene fog color to use at night.
	/// </summary>
	public Color nightFog;
	
	/// <summary>
	/// The calculated time at which dawn occurs based on 1/4 of dayCycleLength.
	/// </summary>
	private float dawnTime; 
	
	/// <summary>
	/// The calculated time at which day occurs based on 1/4 of dayCycleLength.
	/// </summary>
	private float dayTime;
	
	/// <summary>
	/// The calculated time at which dusk occurs based on 1/4 of dayCycleLength.
	/// </summary>
	private float duskTime;
	
	/// <summary>
	/// The calculated time at which night occurs based on 1/4 of dayCycleLength.
	/// </summary>
	private float nightTime;
	
	/// <summary>
	/// One quarter the value of dayCycleLength.
	/// </summary>
	private float quarterDay;
	
	//Would be the amount of time remaining in the skybox transition if UpdateSkybox were used.
	//private float remainingTransition;
	
	/// <summary>
	/// The specified intensity of the directional light, if one exists. This value will be
	/// faded to 0 during dusk, and faded from 0 back to this value during dawn.
	/// </summary>
	private float lightIntensity = 0;

    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start()
	{
		Initialize ();

		//Instantiate the target
		Vector3 position = new Vector3 (Random.Range (208, 520), 3f, Random.Range (230, 540));
		//target = (GameObject)GameObject.Instantiate(TargetPrefab, position, Quaternion.identity);


		//Instantiate the seeker
		//position = new Vector3 (470, 1f, 250);

		//make leaders
        //only ended up using one leader though
        for (int i = 0; i < numOfLeaders; i++) {
            position = new Vector3 (Random.Range (208, 465), 0.3f, Random.Range (230, 500));
            GameObject leader = (GameObject)GameObject.Instantiate(LeaderPrefab, position, Quaternion.identity);
            leader.name =  "Leader " + i;
            leader.GetComponent<Vehicle>().isWandering = true;
            leaders.Add(leader);
        }


        //make flockers
        for (int i = 0; i < numOfFlockers; i++) {
            position = new Vector3(Random.Range (445, 480), 1f, Random.Range (238, 280));
            position = new Vector3(Random.Range (445, 480), 1f, Random.Range (238, 280));
            GameObject guy = (GameObject)GameObject.Instantiate(GuyPrefab, position, Quaternion.identity);
            guy.name =  "dude " + i;
            guy.GetComponent<Seeker>().myTarget = null;
            guy.GetComponent<Seeker>().myLeader = leaders[0];
            flock.Add (guy);
        }

		//make some scary wandering guys
        for (int i = 0; i < numOfWanderingScary; i++) {
            position = new Vector3 (Random.Range (208, 465), 1f, Random.Range (230, 500));
            GameObject scary = (GameObject)GameObject.Instantiate(ScaryPrefab, position, Quaternion.identity);
            scary.name =  "Scary " + i;
            scary.GetComponent<Vehicle>().isWandering = true;
            scary.GetComponent<Wanderer>().myEvadeTarget = leaders[0];
        }


		//Telling the camera to follow the myGuy GameObject
		//Camera.main.GetComponent<SmoothFollow>().target = myGuy.transform;
		//used this instead of the above line, as it was not recognizing it as the main camera
		//cameras [2].GetComponent<SmoothFollow> ().target = myGuy.transform;
        cameras [2].GetComponent<SmoothFollow> ().target = flock[0].transform;

	}	
	
	/// <summary>
	/// Initializes working variables and performs starting calculations.
	/// </summary>
	void Initialize()
	{
        //create a list of flockers
        flock = new List<GameObject> ();
        leaders = new List<GameObject>();
        modFlock = new List<GameObject>();

        screenFader = GameObject.Find("ScreenFader").GetComponent<Image>();
        screenFader.color = new Color(60, 0, 0, 0);
        notFaded = true;
        FadeScreen = false;

        currentCameraIndex = 0;
        //Turn all cameras off, except the first default one
        for (int i=1; i<cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        
        //If any cameras were added to the controller, enable the first one
        if (cameras.Length>0)
        {
            cameras [0].gameObject.SetActive (true);
            Debug.Log ("Camera with name: " + cameras [0].GetComponent<Camera>().name + ", is now enabled");
        }

		lightsOn = true;
		dayCycleLength = 180.0f;
		//skyTransitionTime = 3.0f; //would be set if UpdateSkybox were used.
		hoursPerDay = 24.0f;
		dawnTimeOffset = 0.0f;
		quarterDay = dayCycleLength * 0.25f;
		dawnTime = 5.0f;
		dayTime = dawnTime + quarterDay;
		duskTime = dayTime + quarterDay;
		nightTime = duskTime + quarterDay;
	}

    /// <summary>
	/// Update is called once per frame
    /// </summary>
	void Update()
	{
        //if main player is killed fade screen and move them to start to respawn
        if (FadeScreen)
        {
            Color myColor = new Color(60, 0, 0);
            if(notFaded)
            {
                screenFader.color = Color.Lerp(screenFader.color, myColor, fadeSpeed * Time.deltaTime);
            }

            if(screenFader.color.a > 0.9)
            {
                notFaded = false;
                player.transform.position = new Vector3(529f, 1.5f, 276f);
            }

            if(screenFader.color.a > 0 && !notFaded)
            {
                screenFader.color = Color.Lerp(screenFader.color, Color.clear, fadeSpeed * Time.deltaTime);
            }

            if(screenFader.color.a == 0)
            {
                FadeScreen = false;
                notFaded = true;
            }
        }

		if (Input.GetKeyDown(KeyCode.C))
		{
			currentCameraIndex ++;
			Debug.Log ("C button has been pressed. Switching to the next camera");
			if (currentCameraIndex < cameras.Length)
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);
				Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
			}
			else
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
				Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
			}
		}

		// Rudementary phase-check algorithm:
		if (currentCycleTime > nightTime && currentPhase == DayPhase.Dusk) {
			lightsOn = true;
			currentPhase = DayPhase.Night;
		} else if (currentCycleTime > duskTime && currentPhase == DayPhase.Day) {
			lightsOn = true;
			currentPhase = DayPhase.Dusk;
		} else if (currentCycleTime > dayTime && currentPhase == DayPhase.Dawn) {
			lightsOn = false;
			currentPhase = DayPhase.Day;
		} else if (currentCycleTime > dawnTime && currentCycleTime < dayTime && currentPhase == DayPhase.Night) {
			lightsOn = true;
			currentPhase = DayPhase.Dawn;
		}
		
		// Perform standard updates:
		UpdateWorldTime();
		UpdateDaylight();
		UpdateFog();
		//UpdateSkybox(); //would be called if UpdateSkybox were used.
		
		// Update the current cycle time:
		currentCycleTime += Time.deltaTime;
		currentCycleTime = currentCycleTime % dayCycleLength;

		if (lightsOn) 
		{
			CarouselLight.intensity = 5.5f;
			CarouselLight.range = 40;
			flashLight.intensity = 2;
			flashLight.range = 40;
			HurCarLight1.intensity = 5;
			HurCarLight2.intensity = 5;
			HurCarLight3.intensity = 5;
			HurCarLight4.intensity = 5;
			HurCarLight5.intensity = 5;
			HurRideLight.intensity = 4;
            TeaCupLight.intensity = 5;
            RoundUpLight.intensity = 5;
		} 
		else 
		{
			CarouselLight.intensity = 0;
			CarouselLight.range = 0;
			flashLight.intensity = 0;
			flashLight.range = 0;
			HurCarLight1.intensity = 0;
			HurCarLight2.intensity = 0;
			HurCarLight3.intensity = 0;
			HurCarLight4.intensity = 0;
			HurCarLight5.intensity = 0;
			HurRideLight.intensity = 0;
            TeaCupLight.intensity = 0;
            RoundUpLight.intensity = 0;
		}

        if(modFlock != null && modFlock.Count > 0)
        {
            foreach (GameObject food in modFlock)
            {
                //remove guy
                flock.Remove(food);
                modFlock.Remove(food);
                Destroy(food); 
                Vector3 position = new Vector3(Random.Range (445, 480), 1f, Random.Range (238, 280));
                GameObject guy = (GameObject)GameObject.Instantiate(GuyPrefab, position, Quaternion.identity);
                guy.name =  "dude " + Random.Range(10,100);
                guy.GetComponent<Seeker>().myTarget = null;
                guy.GetComponent<Seeker>().myLeader = leaders[0];
                flock.Add (guy);
            }
        }

	}//end update

    /// <summary>
    /// calculate the center of a flock
    /// </summary>
    private void CalcCentroid()
    {
        centroid = Vector3.zero;
        
        //add all flockers location
        foreach (GameObject f in flock) {
            centroid += f.transform.position;
        }
        centroid /= flock.Count;
        gameObject.transform.position = centroid;
    }//end calcCentroid

    /// <summary>
    /// calculate the flock direction
    /// </summary>
    private void CalcFlockDirection()
    {
        flockDirection = Vector3.zero;
        
        //add all flockers forward direction
        foreach (GameObject f in flock) {
            flockDirection += f.transform.forward;
        }
        flockDirection /= flock.Count;
        gameObject.transform.forward = flockDirection;
    }//end calc flock direction


	/// <summary>
	/// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.
	/// </summary>
	void Reset()
	{
		dayCycleLength = 300.0f;
		//skyTransitionTime = 3.0f; //would be set if UpdateSkybox were used.
		hoursPerDay = 24.0f;
		dawnTimeOffset = 0.0f;
		fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
		fullLight = new Color(100.0f / 255.0f, 100.0f / 255.0f, 90.0f / 255.0f);
		dawnDuskFog = new Color(100.0f / 255.0f, 100.0f / 255.0f, 90.0f / 255.0f);
		dayFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
		nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
		Skybox[] skyboxes = Resources.FindObjectsOfTypeAll(typeof(Skybox)) as Skybox[];
		foreach (Skybox box in skyboxes)
		{
			if (box.name == "DawnDusk Skybox")
			{ dawnDuskSkybox = box.material; }
			else if (box.name == "StarryNight Skybox")
			{ nightSkybox = box.material; }
			else if (box.name == "Sunny2 Skybox")
			{ daySkybox = box.material; }
		}
	}


	/// <summary>
	/// Sets the currentPhase to Dawn, turning on the directional light, if any.
	/// </summary>
	public void SetDawn()
	{
		RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		if (GetComponent<Light>() != null)
		{ GetComponent<Light>().enabled = true; }
		currentPhase = DayPhase.Dawn;
	}
	
	/// <summary>
	/// Sets the currentPhase to Day, ensuring full day color ambient light, and full
	/// directional light intensity, if any.
	/// </summary>
	public void SetDay()
	{
		RenderSettings.skybox = daySkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		RenderSettings.ambientLight = fullLight;
		if (GetComponent<Light>() != null)
		{ GetComponent<Light>().intensity = lightIntensity; }
		currentPhase = DayPhase.Day;
	}
	
	/// <summary>
	/// Sets the currentPhase to Dusk.
	/// </summary>
	public void SetDusk()
	{
		RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		currentPhase = DayPhase.Dusk;
	}
	
	/// <summary>
	/// Sets the currentPhase to Night, ensuring full night color ambient light, and
	/// turning off the directional light, if any.
	/// </summary>
	public void SetNight()
	{
		RenderSettings.skybox = nightSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		RenderSettings.ambientLight = fullDark;
		if (GetComponent<Light>() != null)
		{ GetComponent<Light>().enabled = false; }
		currentPhase = DayPhase.Night;
	}
	
	/// <summary>
	/// If the currentPhase is dawn or dusk, this method adjusts the ambient light color and direcitonal
	/// light intensity (if any) to a percentage of full dark or full light as appropriate. Regardless
	/// of currentPhase, the method also rotates the transform of this component, thereby rotating the
	/// directional light, if any.
	/// </summary>
	private void UpdateDaylight()
	{
		if (currentPhase == DayPhase.Dawn)
		{
			float relativeTime = currentCycleTime - dawnTime;
			RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime / quarterDay);
			if (GetComponent<Light>() != null)
			{ GetComponent<Light>().intensity = lightIntensity * (relativeTime / quarterDay); }
		}
		else if (currentPhase == DayPhase.Dusk)
		{
			float relativeTime = currentCycleTime - duskTime;
			RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime / quarterDay);
			if (GetComponent<Light>() != null)
			{ GetComponent<Light>().intensity = lightIntensity * ((quarterDay - relativeTime) / quarterDay); }
		}
		
		transform.Rotate(Vector3.up * ((Time.deltaTime / dayCycleLength) * 360.0f), Space.Self);
	}
	
	/// <summary>
	/// Interpolates the fog color between the specified phase colors during each phase's transition.
	/// eg. From DawnDusk to Day, Day to DawnDusk, DawnDusk to Night, and Night to DawnDusk
	/// </summary>
	private void UpdateFog()
	{
		if (currentPhase == DayPhase.Dawn)
		{
			float relativeTime = currentCycleTime - dawnTime;
			RenderSettings.fogColor = Color.Lerp(dawnDuskFog, dayFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Day)
		{
			float relativeTime = currentCycleTime - dayTime;
			RenderSettings.fogColor = Color.Lerp(dayFog, dawnDuskFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Dusk)
		{
			float relativeTime = currentCycleTime - duskTime;
			RenderSettings.fogColor = Color.Lerp(dawnDuskFog, nightFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Night)
		{
			float relativeTime = currentCycleTime - nightTime;
			RenderSettings.fogColor = Color.Lerp(nightFog, dawnDuskFog, relativeTime / quarterDay);
		}
	}//end update fog
	
	/// <summary>
	/// Updates the World-time hour based on the current time of day.
	/// </summary>
	private void UpdateWorldTime()
	{
		worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
	}
	
    /// <summary>
    /// Enum for the day phase
    /// </summary>
	public enum DayPhase
	{
		Night = 0,
		Dawn = 1,
		Day = 2,
		Dusk = 3
	}

    /// <summary>
    /// is it night?
    /// </summary>
	public bool IsNight()
	{
		if (currentPhase == DayPhase.Night)
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}
}//end class

/// <summary>
/// Implements a Day/Night cycle relative to the game world, with a World-Time clock, and optional Direcitonal Light control.
/// </summary>
/// <!-- 
/// Version 0.0.1.0 (beta)
/// By Reed Kimble
/// Last Revision 5/19/2011
/// -->
/// <remarks>
/// Add this script to a new GameObject to create a Day/Night cycle for the scene. The day/night cycle effect is achieved by modifying the
/// scene ambient light color, fog color, and skybox material.  The script will also rotate, fade, and enable/disable a directional
/// light if one is attached to the same GameObject as the DayNightController script.  The length of a complete day (in seconds) and the number of
/// hours per day are modifiable in the script fields and allow calculation of the World-time hour-of-day.  Each 'phase' of the day is considered
/// to be 1/4 of the dayCycleLength.
/// 
/// Note that the script will rotate the GameObject transform it is attached to, even if no directional light is attached. You will probably want to 
/// use a dedicated GameObject for this script and any attached directional light.
/// 
/// The GameObject with this script should be placed roughly in the center of your scene, with a height of about 2/3 the length (or width) of the terrain.
/// If that GameObject has a light, it should be a directional light pointed straight down (x:90, y:0, z:0).  This light will then be rotated around its
/// x-axis (relative to the scene; eg. as if you used the rotation tool locked on the green x axis) and will reach its horizontal peeks during the
/// end of dusk and beginning of dawn, turning off during the night (upside-down rotation).
/// 
/// The reset command will attempt to use the default skybox assets DawnDusk, Sunny2, and StarryNight if that package has been imported.  The
/// command will also choose acceptable color values and set the day cycle to two minutes. It is suggested that the directional light be a light-
/// yellow or peach in color with a roughly 0.33f intensity.  The script will not set any default values for the light, if one exists, so the light
/// must be configured manually.
/// </remarks>
