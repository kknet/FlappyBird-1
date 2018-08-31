﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This enum contains all the possible GameStates required in the game.
public enum STATE 
{
	START, PLAY, PAUSE, DEAD
}

public class GameState : MonoBehaviour 
{
	[Header("NEEDED COMPONENTS")]
	[Tooltip("This variable should be set to the players Rigidbody2D.")]
	public Rigidbody2D playerRb2d;
	
	[Tooltip("This should be set to the UIManager script of the UI GameObject.")]
	public UIManager uiController;
	
	[Tooltip("This float variable will eventually tell the top and bottom death points.")]
	public float deathPositionX;
	
	[Tooltip("This variable will tell the width of the lines for debugging the top and bottom death points.")]
	public float deathPositionXDebugLineWidth;

	[Header("MODIFIERS")]
	[Tooltip("The game state will start from the game state enum that is set here.")]
	public STATE state;

	[Header("OPTIONS")]
	[Tooltip("Toggle on/off debugging generated by this, script.")]
	[SerializeField]
	private bool toggleDebug;
	
	// This float variable will keep track of the game's score.
	private int score;

	void OnEnable()
	{
		// This will detect if one of these components have not been set...
		if (!playerRb2d || !uiController)
		{
			// ...and if not it adds a notification to the console and...
			debugLog("Player's Rigidbody2D or the UI's UIManager was not set in inspector. Trying to locate the missing GameObjects...");
			// ...tries to locate the player object and set it to match the scripts variable.
			playerRb2d = GameObject.Find("Player").GetComponent<Rigidbody2D>();		 
			if (playerRb2d)
			{
				//If it was found it will notify about this in console.
				debugLog("A GameObject named Player was found and its Rigidbody2D is now being used by the GameState script.");
			}
			// ...tries to locate the UI object and set it to match the scripts variable.
			uiController = GameObject.Find("UI").GetComponent<UIManager>();
			if (uiController)
			{
				//If it was found it will notify about this in console.
				debugLog("A GameObject named UI was found and its UIManager is now being used by the GameState script.");
			}
		}
		// If the safeguards above fail to locate the components, the script will automatically remove the GameState component as it won't work without them.
		// This will also add an line to the console saying that the component will be removed.
		else
		{
			debugLog("Player GameObject has not been added to the current scene. GameState component will be removed.");
			Destroy(gameObject.GetComponent<GameState>());
		}
		
		// This will kick off the score counting method.
		StartCoroutine(scoreCounter());
	}

	void Update()
	{
		debugPositions();

		// The previously mentioned GameState switch case. It will make sure that the right sequence is playing.
		switch(state)
		{
			case STATE.START:
				break;
			case STATE.PLAY:
				uiController.setUISequence(state);
				Time.timeScale = 1;
				uiController.setScoreText(score.ToString());
				if (isDead())
				{
					state = STATE.DEAD;
					debugLog("Player is dead!");
				}
				break;
			case STATE.PAUSE:
				uiController.setScoreText("| |");
				Time.timeScale = 0;
				uiController.setUISequence(state);
				break;
			case STATE.DEAD:
				uiController.setUISequence(state);
				break;
		}
	}

	// Checks if the player is dead.
	public bool isDead()
	{
		if(Mathf.Abs(playerRb2d.position.y) > deathPositionX || state == STATE.DEAD)
		{
			return true;
		}
		return false;
	}

	// This method will allow setting the death state.
	public void setDead()
	{
		state = STATE.DEAD;
	}

	// This methods takes in a string value and prints it into console if textDebugging is enabled.
	void debugLog(string text)
	{
		if (toggleDebug)
		{
			Debug.Log(text);
		}
	}

	void debugPositions()
	{
		// Debug deathzone if player is alive and debugging is toggled on.
		if (!isDead() && toggleDebug)
		{
			Debug.DrawLine(new Vector3(deathPositionXDebugLineWidth, deathPositionX, 0), new Vector3(-deathPositionXDebugLineWidth, deathPositionX, 0), Color.red);
			Debug.DrawLine(new Vector3(deathPositionXDebugLineWidth, -deathPositionX, 0), new Vector3(-deathPositionXDebugLineWidth, -deathPositionX, 0), Color.red);
		}
	}

	// Keeps count of the score.
	IEnumerator scoreCounter()
	{
		while(true)
		{
			score += 1;
			yield return new WaitForSeconds(1f);
		}
	}
}
