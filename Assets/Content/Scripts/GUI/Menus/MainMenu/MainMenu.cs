using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public MenuPositions Menus = new MenuPositions ();

	private Color fade;

	private Texture2D newBackground;

	private DisplayTextureFullScreen background;

	// CHARACTER PREFABS
	[SerializeField] private GameObject akemiHomura;
	[SerializeField] private GameObject kanameMadoka;
	[SerializeField] private GameObject mikiSayaka;
	[SerializeField] private GameObject sakuraKyoko;
	[SerializeField] private GameObject tomoeMami;
	public GameObject selectedCharacter;

	// CURRENT MENU VARIABLES
	private enum MenuState
	{
		MainMenu,
		Leaderboard,
		HelpMenu1,
		HelpMenu2,
		CharacterSelect,
		homura,
		Madoka,
		Sayaka,
		Kyoko,
		Mami,
		loadingScreen
	}
	private MenuState menuState = MenuState.MainMenu;

	private bool mainMenu 			= true;
	private bool leaderboard		= false;
	private bool helpMenu1			= false;
	private bool helpMenu2			= false;
	private bool characterSelect 	= false;
	private bool homura				= false;
	private bool madoka				= false;
	private bool sayaka 			= false;	
	private bool kyoko				= false;
	private bool mami 				= false;
	private bool loadingScreen		= false;

// POSITIONS AND OBJECTS FOR MENUS
	[System.Serializable]
	public class MenuPositions
	{
		public MainMenu mainMenu = new MainMenu ();
		public HelpMenu1 helpMenu1 = new HelpMenu1 ();
		public HelpMenu2 helpMenu2 = new HelpMenu2 ();
		public Leaderboard leaderboard = new Leaderboard ();
		public CharacterSelect characterSelect = new CharacterSelect ();
		public LoadingScreen loadingScreen = new LoadingScreen ();

		[System.Serializable]
		public class MainMenu
		{
			public Texture2D Background;
			public GUISkin guiSkin;

			public _GUIClasses.Location startButton = new _GUIClasses.Location ();
			public _GUIClasses.Location leaderboardButton = new _GUIClasses.Location ();
			public _GUIClasses.Location helpButton = new _GUIClasses.Location ();
			public _GUIClasses.Location exitButton = new _GUIClasses.Location ();

			public void UpdateLocation ()
			{
				startButton.updateLocation ();
				leaderboardButton.updateLocation ();
				helpButton.updateLocation ();
				exitButton.updateLocation ();
			}
		}

		[System.Serializable]
		public class HelpMenu1
		{
			public GUISkin guiSkin;
			public Texture2D Background;

			public _GUIClasses.Location nextButton = new _GUIClasses.Location ();
			public _GUIClasses.Location backButton = new _GUIClasses.Location ();

			public void UpdateLocation ()
			{
				nextButton.updateLocation ();
				backButton.updateLocation ();
			}
		}

		[System.Serializable]
		public class HelpMenu2
		{
			public GUISkin guiSkin;
			public Texture2D Background;

			public _GUIClasses.Location backButton = new _GUIClasses.Location ();
			
			public void UpdateLocation ()
			{
				backButton.updateLocation ();
			}
		}

		[System.Serializable]
		public class Leaderboard
		{
			public GUISkin guiSkin;
			public Texture2D Background;

			public _GUIClasses.Location returnButton = new _GUIClasses.Location ();

			public _GUIClasses.Location nameLable = new _GUIClasses.Location ();
			public _GUIClasses.Location levelLabel = new _GUIClasses.Location ();
			public _GUIClasses.Location scoreLabel = new _GUIClasses.Location ();

			public _GUIClasses.Location newNameText = new _GUIClasses.Location ();
			public _GUIClasses.Location newScoreButton = new _GUIClasses.Location ();

			public _GUIClasses.Location nameLabels = new _GUIClasses.Location ();
			public _GUIClasses.Location levelLabels = new _GUIClasses.Location ();
			public _GUIClasses.Location scoreLabels = new _GUIClasses.Location ();

			public float offset;

			public int leaderboardSize;
			[HideInInspector] public string[] names;
			[HideInInspector] public int[] levels;
			[HideInInspector] public int[] scores;

			[HideInInspector] public string newName;
			[HideInInspector] public int newLevel;
			[HideInInspector] public int newScore;

			[HideInInspector] public bool newHighscore = false;

			public void UpdateLocation ()
			{
				nameLable.updateLocation ();
				levelLabel.updateLocation ();
				scoreLabel.updateLocation ();
				newNameText.updateLocation ();
				newScoreButton.updateLocation ();
				nameLabels.updateLocation ();
				levelLabels.updateLocation ();
				scoreLabels.updateLocation ();
			}

			public void GetScores ()
			{
				names = new string[leaderboardSize];
				levels = new int[leaderboardSize];
				scores = new int[leaderboardSize];

				if (PlayerPrefs.HasKey ("NewLevel"))
				{
					newLevel = PlayerPrefs.GetInt ("NewLevel");
				}
				else
				{
					PlayerPrefs.SetInt ("NewLevel", 0);
					newLevel = 0;
				}

				if (PlayerPrefs.HasKey ("NewScore"))
				{
					newScore = PlayerPrefs.GetInt ("NewScore");
				}
				else 
				{
					PlayerPrefs.SetInt ("NewScore", 0);
					newScore = 0;
				}

				for (int i = 0; i < leaderboardSize; i++)
				{
					if (PlayerPrefs.HasKey ("name" + i))
					{
						names[i] = PlayerPrefs.GetString ("name" + i);
					}
					else 
					{
						PlayerPrefs.SetString ("name" + i, "N/A");
						names[i] = "N/A";
					}

					if (PlayerPrefs.HasKey ("level" + i))
					{
						levels[i] = PlayerPrefs.GetInt ("level" + i);
					}
					else 
					{
						PlayerPrefs.SetInt ("level" + i, 0);
						levels[i] = 0;
					}

					if (PlayerPrefs.HasKey ("score" + i))
					{
						scores[i] = PlayerPrefs.GetInt ("score" + i);
					}
					else
					{
						PlayerPrefs.SetInt ("score" + i, 0);
						scores[i] = 0;
					}
				}
			}

			public void CheckScores ()
			{
				if (newLevel > levels[leaderboardSize - 1])
				{
					newHighscore = true;
				}
				else if (newLevel == levels[leaderboardSize - 1])
				{
					if (newScore > levels[leaderboardSize - 1])
					{
						newHighscore = true;
					}
				}
			}

			public void SetNewScores ()
			{
				int[] tempLevel = new int[leaderboardSize];
				int[] tempScore = new int[leaderboardSize];
				string[] tempName = new string[leaderboardSize];
				bool higherLevel = false;

				for (int i = 0; i < leaderboardSize; i++)
				{
					if (newLevel >= levels[i])
					{
						if (newLevel > levels[i])
							higherLevel = true;
						else
							higherLevel = false;

						if (newScore > scores[i] || higherLevel)
						{
							for (int t = 0; t < leaderboardSize; t++)
							{
								tempLevel[t] = levels[t];
								tempScore[t] = scores[t];
								tempName[t] = names[t];
							}

							for (int n = i; n < leaderboardSize; n++)
							{
								if (n + 1 < leaderboardSize)
								{
									levels[n + 1] = tempLevel[n];
									scores[n + 1] = tempScore[n];
									names[n + 1] = tempName[n];

									PlayerPrefs.SetInt ("level" + (n + 1), tempLevel[n]);
									PlayerPrefs.SetInt ("score" + (n + 1), tempScore[n]);
									PlayerPrefs.SetString ("name" + (n + 1), tempName[n]);
								}
							}
							levels[i] = newLevel;
							scores[i] = newScore;
							names[i] = newName;

							PlayerPrefs.SetInt ("level" + i, newLevel);
							PlayerPrefs.SetInt ("score" + i, newScore);
							PlayerPrefs.SetString ("name" + i, newName);

							PlayerPrefs.SetInt ("NewLevel", 0);
							PlayerPrefs.SetInt ("NewScore", 0);

							newName = "";
							newHighscore = false;
							break;
						}
					}
				}
			}
		}

		[System.Serializable]
		public class CharacterSelect
		{
			public CharacterSelectMenu characterSelectMenu = new CharacterSelectMenu ();
			public HomuraMenu homuraMenu = new HomuraMenu ();
			public MadokaMenu madokaMenu = new MadokaMenu ();
			public SayakaMenu sayakaMenu = new SayakaMenu ();
			public KyokoMenu kyokoMenu = new KyokoMenu ();
			public MamiMenu mamiMenu = new MamiMenu ();

			[System.Serializable]
			public class CharacterSelectMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location homuraButton = new _GUIClasses.Location ();
				public _GUIClasses.Location madokaButton = new _GUIClasses.Location ();
				public _GUIClasses.Location sayakaButton = new _GUIClasses.Location ();
				public _GUIClasses.Location kyokoButton = new _GUIClasses.Location ();
				public _GUIClasses.Location mamiButton = new _GUIClasses.Location ();

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					homuraButton.updateLocation ();
					madokaButton.updateLocation ();
					sayakaButton.updateLocation ();
					kyokoButton.updateLocation ();
					mamiButton.updateLocation ();
				}
			}

			[System.Serializable]
			public class HomuraMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location lifeTimerLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location speedLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location specialLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location startButton = new _GUIClasses.Location ();

				public Vector2 lifeTimerLabelSize;
				public Vector2 speedLabelSize;
				public Vector2 specialLabelSize;

				public string lifeTimerText;
				public string speedText;
				public string specialText;

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					lifeTimerLabel.updateLocation ();
					speedLabel.updateLocation ();
					specialLabel.updateLocation ();
					startButton.updateLocation ();
				}
			}

			[System.Serializable]
			public class MadokaMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location lifeTimerLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location speedLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location specialLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location startButton = new _GUIClasses.Location ();

				public Vector2 lifeTimerLabelSize;
				public Vector2 speedLabelSize;
				public Vector2 specialLabelSize;

				public string lifeTimerText;
				public string speedText;
				public string specialText;

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					lifeTimerLabel.updateLocation ();
					speedLabel.updateLocation ();
					specialLabel.updateLocation ();
					startButton.updateLocation ();
				}
			}

			[System.Serializable]
			public class SayakaMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location lifeTimerLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location speedLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location specialLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location startButton = new _GUIClasses.Location ();

				public Vector2 lifeTimerLabelSize;
				public Vector2 speedLabelSize;
				public Vector2 specialLabelSize;

				public string lifeTimerText;
				public string speedText;
				public string specialText;

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					lifeTimerLabel.updateLocation ();
					speedLabel.updateLocation ();
					specialLabel.updateLocation ();
					startButton.updateLocation ();
				}
			}

			[System.Serializable]
			public class KyokoMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location lifeTimerLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location speedLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location specialLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location startButton = new _GUIClasses.Location ();

				public Vector2 lifeTimerLabelSize;
				public Vector2 speedLabelSize;
				public Vector2 specialLabelSize;

				public string lifeTimerText;
				public string speedText;
				public string specialText;

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					lifeTimerLabel.updateLocation ();
					speedLabel.updateLocation ();
					specialLabel.updateLocation ();
					startButton.updateLocation ();
				}
			}

			[System.Serializable]
			public class MamiMenu
			{
				public Texture2D Background;
				public GUISkin guiSkin;

				public _GUIClasses.Location returnButton = new _GUIClasses.Location ();
				public _GUIClasses.Location lifeTimerLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location speedLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location specialLabel = new _GUIClasses.Location ();
				public _GUIClasses.Location startButton = new _GUIClasses.Location ();

				public Vector2 lifeTimerLabelSize;
				public Vector2 speedLabelSize;
				public Vector2 specialLabelSize;

				public string lifeTimerText;
				public string speedText;
				public string specialText;

				public void UpdateLocation ()
				{
					returnButton.updateLocation ();
					lifeTimerLabel.updateLocation ();
					speedLabel.updateLocation ();
					specialLabel.updateLocation ();
					startButton.updateLocation ();
				}
			}
		}

		[System.Serializable]
		public class LoadingScreen
		{
			public Texture2D Background;
			
			public float loadTime;
		}
	}

	void Start ()
	{
		background = GetComponent<DisplayTextureFullScreen> ();
		background.graphic.texture = Menus.mainMenu.Background;
		fade.a = 1;
	}
	
	void FixedUpdate ()
	{
		background.GUIColor.a = fade.a;
		UpdateLocation ();
	}

	void UpdateLocation ()
	{
		switch (menuState)
		{
		case MenuState.MainMenu:
			Menus.mainMenu.UpdateLocation ();
			break;
		case MenuState.Leaderboard:
			Menus.leaderboard.UpdateLocation ();
			break;
		case MenuState.HelpMenu1:
			Menus.helpMenu1.UpdateLocation ();
			break;
		case MenuState.HelpMenu2:
			Menus.helpMenu2.UpdateLocation ();
			break;
		case MenuState.CharacterSelect:
			Menus.characterSelect.characterSelectMenu.UpdateLocation ();
			break;
		case MenuState.homura:
			Menus.characterSelect.homuraMenu.UpdateLocation ();
			break;
		case MenuState.Madoka:
			Menus.characterSelect.madokaMenu.UpdateLocation ();
			break;
		case MenuState.Sayaka:
			Menus.characterSelect.sayakaMenu.UpdateLocation ();
			break;
		case MenuState.Kyoko:
			Menus.characterSelect.kyokoMenu.UpdateLocation ();
			break;
		case MenuState.Mami:
			Menus.characterSelect.mamiMenu.UpdateLocation ();
			break;
		}
	}

	void GameOver ()
	{
		StartCoroutine (FadeInLoad (3));
	}

	void ChangeMenu ()
	{
		switch (menuState)
		{
		case MenuState.MainMenu:
			mainMenu = true;
			break;
		case MenuState.Leaderboard:
			leaderboard = true;
			break;
		case MenuState.HelpMenu1:
			helpMenu1 = true;
			break;
		case MenuState.HelpMenu2:
			helpMenu2 = true;
			break;
		case MenuState.CharacterSelect:
			characterSelect = true;
			break;
		case MenuState.homura:
			homura = true;
			break;
		case MenuState.Madoka:
			madoka = true;
			break;
		case MenuState.Sayaka:
			sayaka = true;
			break;
		case MenuState.Kyoko:
			kyoko = true;
			break;
		case MenuState.Mami:
			mami = true;
			break;
		case MenuState.loadingScreen:
			loadingScreen = true;
			break;
		}
	}

// GUI FUNCTIONS
	void OnGUI ()
	{
		GUI.depth = 0;

		if (mainMenu)
			MainMenuGUI ();
		else if (leaderboard)
			LeaderboardGUI ();
		else if (helpMenu1)
			HelpMenu1GUI ();
		else if (helpMenu2)
			HelpMenu2GUI ();
		else if (characterSelect)
			CharacterSelectGUI ();
		else if (homura)
			homuraGUI ();
		else if (madoka)
			MadokaGUI ();
		else if (sayaka)
			SayakaGUI ();
		else if (kyoko)
			KyokoGUI ();
		else if (mami)
			MamiGUI ();
		else if (loadingScreen)
			LoadingScreenGUI ();
	}

// MAIN MENU GUI
	void MainMenuGUI ()
	{
		GUI.skin = Menus.mainMenu.guiSkin;

	// Start (character select) Button.
		if (GUI.Button (new Rect (Menus.mainMenu.startButton.offset.x + Menus.mainMenu.startButton.position.x, 
		                          Menus.mainMenu.startButton.offset.y + Menus.mainMenu.startButton.position.y, 
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			mainMenu = false;
			menuState = MenuState.CharacterSelect;
		}

	// Leaderboard Button
		if (GUI.Button (new Rect (Menus.mainMenu.leaderboardButton.offset.x + Menus.mainMenu.leaderboardButton.position.x, 
		                          Menus.mainMenu.leaderboardButton.offset.y + Menus.mainMenu.leaderboardButton.position.y, 
		                          250, 50), "Leaderboard"))
		{
			Menus.leaderboard.GetScores ();
			Menus.leaderboard.CheckScores ();

			StartCoroutine (FadeOut (1));
			newBackground = Menus.leaderboard.Background;
			mainMenu = false;
			menuState = MenuState.Leaderboard;
		}

	// Help Button.
		if (GUI.Button (new Rect (Menus.mainMenu.helpButton.offset.x + Menus.mainMenu.helpButton.position.x, 
		                          Menus.mainMenu.helpButton.offset.y + Menus.mainMenu.helpButton.position.y, 
		                          100, 50), "Help"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.helpMenu1.Background;
			mainMenu = false;
			menuState = MenuState.HelpMenu1;
		}

	// Exit Button.
		if (GUI.Button (new Rect (Menus.mainMenu.exitButton.offset.x + Menus.mainMenu.exitButton.position.x, 
		                          Menus.mainMenu.exitButton.offset.y + Menus.mainMenu.exitButton.position.y, 
		                          100, 50), "Exit"))
		{
			Application.Quit ();
		}
	}

// LEADERBOARD GUI
	void LeaderboardGUI ()
	{
		GUI.skin = Menus.leaderboard.guiSkin;

		if (Menus.leaderboard.newHighscore)
		{
			Menus.leaderboard.newName = GUI.TextField (new Rect (Menus.leaderboard.newNameText.offset.x + Menus.leaderboard.newNameText.position.x,
			                                                     Menus.leaderboard.newNameText.offset.y + Menus.leaderboard.newNameText.position.y,
			                                                     300, 50), Menus.leaderboard.newName, 10);

			if (GUI.Button (new Rect (Menus.leaderboard.newScoreButton.offset.x + Menus.leaderboard.newScoreButton.position.x, 
			                          Menus.leaderboard.newScoreButton.offset.y + Menus.leaderboard.newScoreButton.position.y, 
			                          100, 50), "Enter"))
			{
				Menus.leaderboard.SetNewScores ();
			}
		}
		else 
		{
			GUI.Label (new Rect (Screen.width / 2 - 50, 0, 400, 50), "Leaderboard");
			GUI.Label (new Rect (Menus.leaderboard.nameLable.offset.x + Menus.leaderboard.nameLable.position.x,
			                     Menus.leaderboard.nameLable.offset.y + Menus.leaderboard.nameLable.position.y,
			                     150, 50), "Name");
			GUI.Label (new Rect (Menus.leaderboard.levelLabel.offset.x + Menus.leaderboard.levelLabel.position.x,
			                     Menus.leaderboard.levelLabel.offset.y + Menus.leaderboard.levelLabel.position.y,
			                     150, 50), "Level");
			GUI.Label (new Rect (Menus.leaderboard.scoreLabel.offset.x + Menus.leaderboard.scoreLabel.position.x,
			                     Menus.leaderboard.scoreLabel.offset.y + Menus.leaderboard.scoreLabel.position.y,
			                     150, 50), "Score");

			float offset = 0;
			for (int i = 0; i < Menus.leaderboard.leaderboardSize; i++)
			{
				GUI.Box (new Rect (Menus.leaderboard.nameLabels.offset.x + Menus.leaderboard.nameLabels.position.x,
				                   Menus.leaderboard.nameLabels.offset.y + (Menus.leaderboard.nameLabels.position.y + offset),
				                   300, 50), "" + (i + 1) + ". " + Menus.leaderboard.names[i]);
				GUI.Box (new Rect (Menus.leaderboard.levelLabels.offset.x + Menus.leaderboard.levelLabels.position.x,
				                   Menus.leaderboard.levelLabels.offset.y + (Menus.leaderboard.levelLabels.position.y + offset),
				                   300, 50), "" + Menus.leaderboard.levels[i]);
				GUI.Box (new Rect (Menus.leaderboard.scoreLabels.offset.x + Menus.leaderboard.scoreLabels.position.x,
				                   Menus.leaderboard.scoreLabels.offset.y + (Menus.leaderboard.scoreLabels.position.y + offset),
				                   300, 50), "" + Menus.leaderboard.scores[i]);
				offset += Menus.leaderboard.offset;
			}

			if (GUI.Button (new Rect (Menus.leaderboard.returnButton.offset.x + Menus.leaderboard.returnButton.position.x,
			                          Menus.leaderboard.returnButton.offset.y + Menus.leaderboard.returnButton.position.y,
			                          150, 50), "Return"))
			{
				StartCoroutine (FadeOut (1));
				newBackground = Menus.mainMenu.Background;
				leaderboard= false;
				menuState = MenuState.MainMenu;
			}
		}
	}

// HELP MENU 1 GUI
	void HelpMenu1GUI ()
	{
		GUI.skin = Menus.helpMenu1.guiSkin;

		if (GUI.Button (new Rect (Menus.helpMenu1.backButton.offset.x + Menus.helpMenu1.backButton.position.x,
		                          Menus.helpMenu1.backButton.offset.y + Menus.helpMenu1.backButton.position.y,
		                          100, 50), "Back"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.mainMenu.Background;
			helpMenu1 = false;
			menuState = MenuState.MainMenu;
		}

		if (GUI.Button (new Rect (Menus.helpMenu1.nextButton.offset.x + Menus.helpMenu1.nextButton.position.x,
		                          Menus.helpMenu1.nextButton.offset.y + Menus.helpMenu1.nextButton.position.y,
		                          100, 50), "Next"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.helpMenu2.Background;
			helpMenu1 = false;
			menuState = MenuState.HelpMenu2;
		}
	}

// HELP MENU 2 GUI
	void HelpMenu2GUI ()
	{
		GUI.skin = Menus.helpMenu2.guiSkin;
		
		if (GUI.Button (new Rect (Menus.helpMenu2.backButton.offset.x + Menus.helpMenu2.backButton.position.x,
		                          Menus.helpMenu2.backButton.offset.y + Menus.helpMenu2.backButton.position.y,
		                          100, 50), "Back"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.helpMenu1.Background;
			helpMenu2 = false;
			menuState = MenuState.HelpMenu1;
		}
	}

// CHARACTER SELECTION GUI
	void CharacterSelectGUI ()
	{
		GUI.skin = Menus.characterSelect.characterSelectMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.returnButton.offset.x + Menus.characterSelect.characterSelectMenu.returnButton.position.x,
		                          Menus.characterSelect.characterSelectMenu.returnButton.offset.y + Menus.characterSelect.characterSelectMenu.returnButton.position.y, 
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.mainMenu.Background;
			characterSelect = false;
			menuState = MenuState.MainMenu;
		}

	// Homura Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.homuraButton.offset.x + Menus.characterSelect.characterSelectMenu.homuraButton.position.x,
		                          Menus.characterSelect.characterSelectMenu.homuraButton.offset.y + Menus.characterSelect.characterSelectMenu.homuraButton.position.y,
		                          135, 100), "Akemi Homura"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.homuraMenu.Background;
			characterSelect = false;
			menuState = MenuState.homura;
		}

	// Madoka Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.madokaButton.offset.x + Menus.characterSelect.characterSelectMenu.madokaButton.position.x,
		                          Menus.characterSelect.characterSelectMenu.madokaButton.offset.y + Menus.characterSelect.characterSelectMenu.madokaButton.position.y, 
		                          125, 100), "Kaname Madoka"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.madokaMenu.Background;
			characterSelect = false;
			menuState = MenuState.Madoka;
		}

	// Sayaka Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.sayakaButton.offset.x + Menus.characterSelect.characterSelectMenu.sayakaButton.position.x,
		                          Menus.characterSelect.characterSelectMenu.sayakaButton.offset.y + Menus.characterSelect.characterSelectMenu.sayakaButton.position.y, 
		                          125, 100), "Miki Sayaka"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.sayakaMenu.Background;
			characterSelect = false;
			menuState = MenuState.Sayaka;
		}

	// Kyoko Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.kyokoButton.offset.x + Menus.characterSelect.characterSelectMenu.kyokoButton.position.x, 
		                          Menus.characterSelect.characterSelectMenu.kyokoButton.offset.y + Menus.characterSelect.characterSelectMenu.kyokoButton.position.y,
		                          125, 100), "Sakura Kyoko"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.kyokoMenu.Background;
			characterSelect = false;
			menuState = MenuState.Kyoko;
		}
	
	// Mami Button.
		if (GUI.Button (new Rect (Menus.characterSelect.characterSelectMenu.mamiButton.offset.x + Menus.characterSelect.characterSelectMenu.mamiButton.position.x,
		                          Menus.characterSelect.characterSelectMenu.mamiButton.offset.y + Menus.characterSelect.characterSelectMenu.mamiButton.position.y,
		                          125, 100), "Tomoe Mami"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.mamiMenu.Background;
			characterSelect = false;
			menuState = MenuState.Mami;
		}
	}

// HOMURA GUI
	void homuraGUI ()
	{
		GUI.skin = Menus.characterSelect.homuraMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.homuraMenu.returnButton.offset.x + Menus.characterSelect.homuraMenu.returnButton.position.x, 
		                          Menus.characterSelect.homuraMenu.returnButton.offset.y + Menus.characterSelect.homuraMenu.returnButton.position.y,
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			homura = false;
			menuState = MenuState.CharacterSelect;
		}

	// Start Button.
		if (GUI.Button (new Rect (Menus.characterSelect.homuraMenu.startButton.offset.x + Menus.characterSelect.homuraMenu.startButton.position.x,
		                          Menus.characterSelect.homuraMenu.startButton.offset.y + Menus.characterSelect.homuraMenu.startButton.position.y,
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			selectedCharacter = akemiHomura;
			newBackground = Menus.loadingScreen.Background;
			homura = false;
			menuState = MenuState.loadingScreen;
		}

	// Life Timer Label.
		GUI.Label (new Rect (Menus.characterSelect.homuraMenu.lifeTimerLabel.offset.x + Menus.characterSelect.homuraMenu.lifeTimerLabel.position.x,
		                     Menus.characterSelect.homuraMenu.lifeTimerLabel.offset.y + Menus.characterSelect.homuraMenu.lifeTimerLabel.position.y,
		                     Menus.characterSelect.homuraMenu.lifeTimerLabelSize.x, Menus.characterSelect.homuraMenu.lifeTimerLabelSize.y), 
		           Menus.characterSelect.homuraMenu.lifeTimerText);

	// Speed Label.
		GUI.Label (new Rect (Menus.characterSelect.homuraMenu.speedLabel.offset.x + Menus.characterSelect.homuraMenu.speedLabel.position.x,
		                     Menus.characterSelect.homuraMenu.speedLabel.offset.y + Menus.characterSelect.homuraMenu.speedLabel.position.y,
		                     Menus.characterSelect.homuraMenu.speedLabelSize.x, Menus.characterSelect.homuraMenu.speedLabelSize.y),
		           Menus.characterSelect.homuraMenu.speedText);
	// Special Label.
		GUI.Label (new Rect (Menus.characterSelect.homuraMenu.specialLabel.offset.x + Menus.characterSelect.homuraMenu.specialLabel.position.x,
		                     Menus.characterSelect.homuraMenu.specialLabel.offset.y + Menus.characterSelect.homuraMenu.specialLabel.position.y,
		                     Menus.characterSelect.homuraMenu.specialLabelSize.x, Menus.characterSelect.homuraMenu.specialLabelSize.y),
		           Menus.characterSelect.homuraMenu.specialText);
	}

// MADOKA GUI
	void MadokaGUI ()
	{
		GUI.skin = Menus.characterSelect.madokaMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.madokaMenu.returnButton.offset.x + Menus.characterSelect.madokaMenu.returnButton.position.x, 
		                          Menus.characterSelect.madokaMenu.returnButton.offset.y + Menus.characterSelect.madokaMenu.returnButton.position.y,
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			madoka = false;
			menuState = MenuState.CharacterSelect;
		}

	// Start Button.
		if (GUI.Button (new Rect (Menus.characterSelect.madokaMenu.startButton.offset.x + Menus.characterSelect.madokaMenu.startButton.position.x,
		                          Menus.characterSelect.madokaMenu.startButton.offset.y + Menus.characterSelect.madokaMenu.startButton.position.y,
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			selectedCharacter = kanameMadoka;
			newBackground = Menus.loadingScreen.Background;
			madoka = false;
			menuState = MenuState.loadingScreen;
		}

		// Life Timer Label.
		GUI.Label (new Rect (Menus.characterSelect.madokaMenu.lifeTimerLabel.offset.x + Menus.characterSelect.madokaMenu.lifeTimerLabel.position.x,
		                     Menus.characterSelect.madokaMenu.lifeTimerLabel.offset.y + Menus.characterSelect.madokaMenu.lifeTimerLabel.position.y,
		                     Menus.characterSelect.madokaMenu.lifeTimerLabelSize.x, Menus.characterSelect.madokaMenu.lifeTimerLabelSize.y), 
		           Menus.characterSelect.madokaMenu.lifeTimerText);
		
		// Speed Label.
		GUI.Label (new Rect (Menus.characterSelect.madokaMenu.speedLabel.offset.x + Menus.characterSelect.madokaMenu.speedLabel.position.x,
		                     Menus.characterSelect.madokaMenu.speedLabel.offset.y + Menus.characterSelect.madokaMenu.speedLabel.position.y,
		                     Menus.characterSelect.madokaMenu.speedLabelSize.x, Menus.characterSelect.madokaMenu.speedLabelSize.y),
		           Menus.characterSelect.madokaMenu.speedText);
		// Special Label.
		GUI.Label (new Rect (Menus.characterSelect.madokaMenu.specialLabel.offset.x + Menus.characterSelect.madokaMenu.specialLabel.position.x,
		                     Menus.characterSelect.madokaMenu.specialLabel.offset.y + Menus.characterSelect.madokaMenu.specialLabel.position.y,
		                     Menus.characterSelect.madokaMenu.specialLabelSize.x, Menus.characterSelect.madokaMenu.specialLabelSize.y),
		           Menus.characterSelect.madokaMenu.specialText);
	}

// SAYAKA GUI
	void SayakaGUI ()
	{
		GUI.skin = Menus.characterSelect.sayakaMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.sayakaMenu.returnButton.offset.x + Menus.characterSelect.sayakaMenu.returnButton.position.x, 
		                          Menus.characterSelect.sayakaMenu.returnButton.offset.y + Menus.characterSelect.sayakaMenu.returnButton.position.y,
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			sayaka = false;
			menuState = MenuState.CharacterSelect;
		}

	// Start Button.
		if (GUI.Button (new Rect (Menus.characterSelect.sayakaMenu.startButton.offset.x + Menus.characterSelect.sayakaMenu.startButton.position.x,
		                          Menus.characterSelect.sayakaMenu.startButton.offset.y + Menus.characterSelect.sayakaMenu.startButton.position.y,
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			selectedCharacter = mikiSayaka;
			newBackground = Menus.loadingScreen.Background;
			sayaka = false;
			menuState = MenuState.loadingScreen;
		}

		// Life Timer Label.
		GUI.Label (new Rect (Menus.characterSelect.sayakaMenu.lifeTimerLabel.offset.x + Menus.characterSelect.sayakaMenu.lifeTimerLabel.position.x,
		                     Menus.characterSelect.sayakaMenu.lifeTimerLabel.offset.y + Menus.characterSelect.sayakaMenu.lifeTimerLabel.position.y,
		                     Menus.characterSelect.sayakaMenu.lifeTimerLabelSize.x, Menus.characterSelect.sayakaMenu.lifeTimerLabelSize.y), 
		           Menus.characterSelect.sayakaMenu.lifeTimerText);
		
		// Speed Label.
		GUI.Label (new Rect (Menus.characterSelect.sayakaMenu.speedLabel.offset.x + Menus.characterSelect.sayakaMenu.speedLabel.position.x,
		                     Menus.characterSelect.sayakaMenu.speedLabel.offset.y + Menus.characterSelect.sayakaMenu.speedLabel.position.y,
		                     Menus.characterSelect.sayakaMenu.speedLabelSize.x, Menus.characterSelect.sayakaMenu.speedLabelSize.y),
		           Menus.characterSelect.sayakaMenu.speedText);
		// Special Label.
		GUI.Label (new Rect (Menus.characterSelect.sayakaMenu.specialLabel.offset.x + Menus.characterSelect.sayakaMenu.specialLabel.position.x,
		                     Menus.characterSelect.sayakaMenu.specialLabel.offset.y + Menus.characterSelect.sayakaMenu.specialLabel.position.y,
		                     Menus.characterSelect.sayakaMenu.specialLabelSize.x, Menus.characterSelect.sayakaMenu.specialLabelSize.y),
		           Menus.characterSelect.sayakaMenu.specialText);
	}

// KYOKO GUI
	void KyokoGUI ()
	{
		GUI.skin = Menus.characterSelect.kyokoMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.kyokoMenu.returnButton.offset.x + Menus.characterSelect.kyokoMenu.returnButton.position.x, 
		                          Menus.characterSelect.kyokoMenu.returnButton.offset.y + Menus.characterSelect.kyokoMenu.returnButton.position.y,
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			kyoko = false;
			menuState = MenuState.CharacterSelect;
		}

	// Start Button.
		if (GUI.Button (new Rect (Menus.characterSelect.kyokoMenu.startButton.offset.x + Menus.characterSelect.kyokoMenu.startButton.position.x,
		                          Menus.characterSelect.kyokoMenu.startButton.offset.y + Menus.characterSelect.kyokoMenu.startButton.position.y,
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			selectedCharacter = sakuraKyoko;
			newBackground = Menus.loadingScreen.Background;
			kyoko = false;
			menuState = MenuState.loadingScreen;
		}

		// Life Timer Label.
		GUI.Label (new Rect (Menus.characterSelect.kyokoMenu.lifeTimerLabel.offset.x + Menus.characterSelect.kyokoMenu.lifeTimerLabel.position.x,
		                     Menus.characterSelect.kyokoMenu.lifeTimerLabel.offset.y + Menus.characterSelect.kyokoMenu.lifeTimerLabel.position.y,
		                     Menus.characterSelect.kyokoMenu.lifeTimerLabelSize.x, Menus.characterSelect.kyokoMenu.lifeTimerLabelSize.y), 
		           Menus.characterSelect.kyokoMenu.lifeTimerText);
		
		// Speed Label.
		GUI.Label (new Rect (Menus.characterSelect.kyokoMenu.speedLabel.offset.x + Menus.characterSelect.kyokoMenu.speedLabel.position.x,
		                     Menus.characterSelect.kyokoMenu.speedLabel.offset.y + Menus.characterSelect.kyokoMenu.speedLabel.position.y,
		                     Menus.characterSelect.kyokoMenu.speedLabelSize.x, Menus.characterSelect.kyokoMenu.speedLabelSize.y),
		           Menus.characterSelect.kyokoMenu.speedText);
		// Special Label.
		GUI.Label (new Rect (Menus.characterSelect.kyokoMenu.specialLabel.offset.x + Menus.characterSelect.kyokoMenu.specialLabel.position.x,
		                     Menus.characterSelect.kyokoMenu.specialLabel.offset.y + Menus.characterSelect.kyokoMenu.specialLabel.position.y,
		                     Menus.characterSelect.kyokoMenu.specialLabelSize.x, Menus.characterSelect.kyokoMenu.specialLabelSize.y),
		           Menus.characterSelect.kyokoMenu.specialText);
	}

// MAMI GUI
	void MamiGUI ()
	{
		GUI.skin = Menus.characterSelect.mamiMenu.guiSkin;

	// Return Button.
		if (GUI.Button (new Rect (Menus.characterSelect.mamiMenu.returnButton.offset.x + Menus.characterSelect.mamiMenu.returnButton.position.x, 
		                          Menus.characterSelect.mamiMenu.returnButton.offset.y + Menus.characterSelect.mamiMenu.returnButton.position.y,
		                          125, 50), "Return"))
		{
			StartCoroutine (FadeOut (1));
			newBackground = Menus.characterSelect.characterSelectMenu.Background;
			mami = false;
			menuState = MenuState.CharacterSelect;
		}

	// Start Button.
		if (GUI.Button (new Rect (Menus.characterSelect.mamiMenu.startButton.offset.x + Menus.characterSelect.mamiMenu.startButton.position.x,
		                          Menus.characterSelect.mamiMenu.startButton.offset.y + Menus.characterSelect.mamiMenu.startButton.position.y,
		                          100, 50), "Start"))
		{
			StartCoroutine (FadeOut (1));
			selectedCharacter = tomoeMami;
			newBackground = Menus.loadingScreen.Background;
			mami = false;
			menuState = MenuState.loadingScreen;
		}

		// Life Timer Label.
		GUI.Label (new Rect (Menus.characterSelect.mamiMenu.lifeTimerLabel.offset.x + Menus.characterSelect.mamiMenu.lifeTimerLabel.position.x,
		                     Menus.characterSelect.mamiMenu.lifeTimerLabel.offset.y + Menus.characterSelect.mamiMenu.lifeTimerLabel.position.y,
		                     Menus.characterSelect.mamiMenu.lifeTimerLabelSize.x, Menus.characterSelect.mamiMenu.lifeTimerLabelSize.y), 
		           Menus.characterSelect.mamiMenu.lifeTimerText);
		
		// Speed Label.
		GUI.Label (new Rect (Menus.characterSelect.mamiMenu.speedLabel.offset.x + Menus.characterSelect.mamiMenu.speedLabel.position.x,
		                     Menus.characterSelect.mamiMenu.speedLabel.offset.y + Menus.characterSelect.mamiMenu.speedLabel.position.y,
		                     Menus.characterSelect.mamiMenu.speedLabelSize.x, Menus.characterSelect.mamiMenu.speedLabelSize.y),
		           Menus.characterSelect.mamiMenu.speedText);
		// Special Label.
		GUI.Label (new Rect (Menus.characterSelect.mamiMenu.specialLabel.offset.x + Menus.characterSelect.mamiMenu.specialLabel.position.x,
		                     Menus.characterSelect.mamiMenu.specialLabel.offset.y + Menus.characterSelect.mamiMenu.specialLabel.position.y,
		                     Menus.characterSelect.mamiMenu.specialLabelSize.x, Menus.characterSelect.mamiMenu.specialLabelSize.y),
		           Menus.characterSelect.mamiMenu.specialText);
	}

// LOADING SCREEN GUI
	void LoadingScreenGUI ()
	{
		StartCoroutine (FadeOutLoad (1));
		loadingScreen = false;
	}

// FADE COUROTINES
	IEnumerator FadeOut (float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp (alpha, 0.0f, t));
			fade = newColor;
			yield return null;
		}
		background.graphic.texture = newBackground;
		StartCoroutine (FadeIn (aTime));
	}

	IEnumerator FadeIn (float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp (alpha, 1.0f, t));
			fade = newColor;
			yield return null;
		}
		ChangeMenu ();
	}

	IEnumerator FadeInLoad (float aTime)
	{
		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp (alpha, 1.0f, t));
			fade = newColor;
			yield return null;
		}

		Menus.leaderboard.GetScores ();
		Menus.leaderboard.CheckScores ();
		
		StartCoroutine (FadeOut (1));
		newBackground = Menus.leaderboard.Background;
		loadingScreen = false;
		menuState = MenuState.Leaderboard;

		StartCoroutine (FadeOut (1));
	}

	IEnumerator FadeOutLoad (float aTime)
	{
		Application.LoadLevelAdditive ("Default_Stage");
		yield return new WaitForSeconds (Menus.loadingScreen.loadTime);

		float alpha = fade.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp (alpha, 0.0f, t));
			fade = newColor;
			yield return null;
		}
		Camera.main.SendMessage ("GameStart", SendMessageOptions.DontRequireReceiver);
		GameObject.FindGameObjectWithTag ("LevelController").SendMessage ("GameStart");
	}
}





















