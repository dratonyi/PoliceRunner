//Author: Dani Ratonyi
//FileName: MainClass.cs
//Project Name: dani.ratonyi.PASS_3
//Creation Date: May 24th, 2021 
//Modified Date: June 21, 2021
//Description: Plays an infinite runner game with randomized obstacles, multiple levels with increasing difficulty and a complex score system. The player has to survive waves and waves of
//police helicopters cars and barricades. The player also has the option to pick up a surprise in the form of either a buff or debuff but they dont know which one they will get. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGameEngine
{
    class MainClass : AbstractGame
    {
        //Game States - Add/Remove/Modify as needed
        //These are the most common game states, but modify as needed
        //You will ALSO need to modify the two switch statements in Update and Draw
        private const int MENU = 0;
        private const int INSTRUCTIONS = 2;
        private const int GAMEPLAY = 3;
        private const int ENDGAME = 5;

        //Choose between UI_RIGHT, UI_LEFT, UI_TOP, UI_BOTTOM, UI_NONEs
        private static int uiLocation = Helper.UI_BOTTOM;

        ////////////////////////////////////////////
        //Set the game and user interface dimensions
        ////////////////////////////////////////////

        //Min: 5 top/bottom, 10 left/right, Max: 30
        private static int uiSize = 5;

        //On VS: Max: 120 - uiSize, UI_NONE gives full width up to 120
        //On Repl: Max: 80 - uiSize, UI_NONE can give full width up to 80
        private static int gameWidth = 80;

        //On VS: Max: 50 - uiSize, UI_NONE gives full height up to 50
        //On Repl: Max: 24 - uiSize, UI_NONE can give full height up to 24
        private static int gameHeight = 22;

        //Store and set the initial game state, typically MENU to start
        int gameState = MENU;

        ////////////////////////////////////////////
        //Define your Global variables here (They do NOT need to be static)
        ////////////////////////////////////////////

        //constants representing player states
        const int STAND = 0;
        const int DUCK = 1;
        const int JUMP = 2;
        const int DEAD = 3;

        //constants that represent the individual buffs and the total number of buffs that exists
        const int INVINCIBLE_BUFF = 0;
        const int NO_SCORE_BUFF = 1;
        const int PLANE_SHIFT_BUFF = 2;
        const int LOW_GRAVITY_BUFF = 3;
        const int HEALTH_BUFF = 4;      
        const int MINUS_HEALTH_BUFF = 5;
        const int COMBO_KILLER_BUFF = 6; 
        const int NUM_BUFF = 7;

        //cosntant for each menu option that exists
        const int NUM_OPTIONS = 3;
        const int PLAY_OPT = 1;
        const int INSTRUCTIONS_OPT = 2;
        const int EXIT_OPT = 3;

        //constant for the maximum health of the player
        const int MAX_HEALTH = 3;

        //variable for the maximum time the player can spend ducking
        const int MAX_DUCK_TIME = 700;

        //constants related to spawning the obstacles and the chances of spawning the obstacles
        const int FIRST_OBS_CHANCE = 50;
        const float FIRST_OBS_TIME = 1000f;
        const int SHORT_CHANCE = 0;
        const int TALL_CHANCE = 1;
        const int FLYING_CHANCE = 2;

        //constant for the strength of gravity (2 different values are required for balance)
        const float STONE_GRAVITY = 9.8f / 60f;
        const float PLAYER_GRAVITY = 7f / 60f;

        //array of images for the players appearances and array of game objects for the player
        Image[] playerImg = new Image[4];
        GameObject[] player = new GameObject[4];

        //variables for the throwable stone
        Image stoneImg;
        GameObject stone;

        //variable for the obstacles
        Image shortObsImg;
        Image flyingObsImg;
        Image tallObsImg;
        GameObject[] flyingObs = new GameObject[3];
        GameObject[] shortObs = new GameObject[3];
        GameObject[] tallObs = new GameObject[3];

        //variables for the game objects created to display buff/debuff effects
        GameTextObject buffText;
        GameBarObject buffBar;

        //variables for the text based UI elements 
        UITextObject scoreText;
        UITextObject healthText;
        UITextObject comboText;
        UITextObject multiplierText;
        UITextObject highScoreText;
        UITextObject highCongratsText;
        GameTextObject obstacleSpawnText;

        //variables for the buffs/debuffs in the game
        Image buffImg;
        GameObject buff;

        //variables for the bar based UI elements
        UIBarObject healthBar;
        UIBarObject comboBar;
        GameBarObject obstacleBar;        

        //images and game objects
        Image titleImg;
        GameObject title;

        //variables for the images and the gameobjects of the menu buttons
        Image playImg;
        Image settingsImg;
        Image exitImg;
        Image pointerImg;
        GameObject settingsButton;
        GameObject exitButton;
        GameObject playButton;
        GameObject pointer;

        //variable for the contents of the instructions
        Image instructionsImg;
        Image settingsMenuImg;
        GameObject instructionsTxt;
        GameObject settingsMenu;

        //variables for the game over message
        Image gameOverImg;
        GameObject gameOver;

        //variable for the go back to menu text
        UITextObject backToMenuText;

        //variable for the message displayed when the player moves onto the next level
        GameTextObject nextLvlText;
        GameTextObject currentLvlText;
        UITextObject endLevelText;
        GameBarObject levelBar;

        //variable for the width of every bar object in the game
        int barWidth = 30;

        //variables related to the level the player is on
        string nextLevelMsg = "NEXT LEVEL!!!";
        int levelCount = 1;
        float nextLevelTime = 0;
        float maxLevelTime = 2000f;

        //variables for the menu buttons' and the pointer's dimentions 
        int menuButtonY;
        int pointerDiff = 14;
        int menuOption = 1;

        //variable for by how much the flying obstacle is above the ground
        int flyingAboveGround = 2;

        //holds the players current player state
        int playerState = STAND;

        //variable score that starts at 0 and a score incrementer that increases score by 10
        float score = 0;
        float scoreIncrementer = 10f;
        int scoreCombo = 0;
        float scoreMultiplier = 1f;
        float highScore = 0f;
        bool isScoreHigh = true;

        //variable for the players current health set to max health and variable for how much demage an obstacle deals 
        int health = MAX_HEALTH;
        int obstacleDamage = 1;

        //variables all related to the creation of obstacles
        float maxObsTime = FIRST_OBS_TIME;
        float obsTimer = FIRST_OBS_TIME;
        float minObsTime = 500f;
        float obsTimeDecrement = 50f;

        //variables for the chances of spawning obstacles
        int obsChance = FIRST_OBS_CHANCE;
        int obsChancePlus = 5;
        int maxObsChance = 90;
        int randChanceNum;
        int obsGroundPos;
        int obsFlyingPos;

        //variable for the speed of the obstacles 
        float obsSpeed = 0.5f;
        int obsDestroyed = 0; 
        int maxObsDestroyed = 5;

        //timer to chekc if the player has been ducking for 1.5 seconds
        int duckTimer = MAX_DUCK_TIME;

        //the speed at which the player jumps up
        float jumpSpeed = 1.2f;

        //the speed at which the player is currently traveling upwards
        float ySpeed = 0f;

        //variables foe the different characteristics of the projectile
        float throwSpeed = 0.8f;
        float xProjSpeed;
        float yProjSpeed;
        float stoneTimer = 0f;
        float maxStoneTimer = 1000f;

        //variables related to buff/debuff generating
        int buffChance = 90;
        int randomBuff;
        float buffDisTime = 0;
        float maxBuffTime = 3000f;
        bool activeBuff = false;
        bool buffCollision = false;
        bool[] buffEffects = new bool[NUM_BUFF];

        //variable for doubling certain things in the game like gravity or time 
        int generalMultiplier = 2;

        private readonly Random _random = new Random();
        static void Main(string[] args)
        {
            /***************************************************************
                        DO NOT TOUCH THIS SECTION
            ***************************************************************/
            GameContainer gameContainer = new GameContainer(new MainClass(), uiLocation, uiSize, gameWidth, gameHeight);
            gameContainer.Start();
        }

        //Pre: the game container
        //Post: None
        //Desc: Loads images and variables prior to the start of the game (only once)
        public override void LoadContent(GameContainer gc)
        {
            //loads all the player images 
            playerImg[STAND] = Helper.LoadImage("Images/PlayerStand.txt");
            playerImg[DUCK] = Helper.LoadImage("Images/PlayerDuck.txt");
            playerImg[JUMP] = Helper.LoadImage("Images/PlayerJump.txt");
            playerImg[DEAD] = Helper.LoadImage("Images/DeadPlayer.txt");

            //loads the obstacle images
            shortObsImg = Helper.LoadImage("Images/ShortObstacle.txt");
            flyingObsImg = Helper.LoadImage("Images/FlyingObstacle.txt");
            tallObsImg = Helper.LoadImage("Images/TallObstacle.txt");

            //loads the image of the projectlie
            stoneImg = Helper.LoadImage("Images/Bullet.txt");

            //loads the images for the menu and the endgame
            titleImg = Helper.LoadImage("Images/Title.txt");
            playImg = Helper.LoadImage("Images/PlayButton.txt");
            settingsImg = Helper.LoadImage("Images/SettingsButton.txt");
            exitImg = Helper.LoadImage("Images/ExitButton.txt");
            pointerImg = Helper.LoadImage("Images/Pointer.txt");
            gameOverImg = Helper.LoadImage("Images/GameOverText.txt");
            
            //loads the images in the settings part of the game
            instructionsImg = Helper.LoadImage("Images/InstructionsText.txt");
            settingsMenuImg = Helper.LoadImage("Images/SettingsImage.txt");

            //loads the images of the buffs/debuffs
            buffImg = Helper.LoadImage("Images/Buff.txt");

            //loops through all the player game objects and initializes them
            for (int i = 0; i < playerImg.Length; i++)
            {
                //initializes the players image                
                player[i] = new GameObject(gc, playerImg[i], 15, gc.GetGameHeight() - playerImg[i].GetHeight(), true);
            }

            //Calls a subprogram to initialize the obstacles
            ObstacleLoader(gc, shortObs, shortObsImg);
            ObstacleLoader(gc, tallObs, tallObsImg);
            ObstacleLoader(gc, flyingObs, flyingObsImg);

            //initializes the images of the buff/debuffs as well as their indicators and sets bar and text based indicators and sets their position            
            buff = new GameObject(gc, buffImg, 1, 1, false);
            buffBar = new GameBarObject(gc, gc.GetGameWidth() / 2 - barWidth / 2, gc.GetGameHeight() / 2, Helper.MAGENTA, true, (int)maxBuffTime, (int)buffDisTime, barWidth);
            buffText = new GameTextObject(gc, buffBar.GetPos().x, buffBar.GetPos().y - 1, Helper.MAGENTA, true, " ");

            //sets the height of the menu buttons 
            menuButtonY = gc.GetGameHeight() * 1 / 2;

            //initializes the projectiles image            
            stone = new GameObject(gc, stoneImg, player[STAND].GetPos().x + player[STAND].GetWidth(), player[STAND].GetPos().y, false);

            //initializes the title image on the main menu screen            
            title = new GameObject(gc, titleImg, (gc.GetGameWidth() / 2) - (titleImg.GetWidth() / 2), 1, true);

            //initializes  the UI elements about game levels 
            nextLvlText = new GameTextObject(gc, gc.GetGameWidth() / 2 - nextLevelMsg.Length / 2, gc.GetGameHeight() * 1 / 3, Helper.DARK_GREEN, true, nextLevelMsg);
            currentLvlText = new GameTextObject(gc, gc.GetGameWidth() * 2 / 3, 1, Helper.DARK_GREEN, true, "Current level: " + levelCount);
            endLevelText = new UITextObject(gc, 1, 1, Helper.DARK_GREEN, true, "You have reached level " + levelCount);
            endLevelText.SetPosition(gc.GetGameWidth() / 2 - endLevelText.GetWidth() / 2, 1);
            levelBar = new GameBarObject(gc, gc.GetGameWidth() - barWidth - 1, currentLvlText.GetPos().y + 1, Helper.GREEN, true, maxObsDestroyed * levelCount, obsDestroyed, barWidth);

            //initializes the menu buttons and the pointer
            playButton = new GameObject(gc, playImg, (gc.GetGameWidth() / 3) - (playImg.GetWidth() / 2), menuButtonY, true);
            settingsButton = new GameObject(gc, settingsImg, (gc.GetGameWidth() / 2) - (settingsImg.GetWidth() / 2), menuButtonY, true);
            exitButton = new GameObject(gc, exitImg, (gc.GetGameWidth() * 2 / 3) - (exitImg.GetWidth() / 2), menuButtonY, true);
            pointer = new GameObject(gc, pointerImg, gc.GetGameWidth() / 3 - pointerImg.GetWidth() / 2, menuButtonY + settingsImg.GetHeight() + 1, true);

            //initializes the image of the game instructions            
            instructionsTxt = new GameObject(gc, instructionsImg, gc.GetGameWidth() / 2 - instructionsImg.GetWidth() / 2, gc.GetGameHeight() * 3/4 - instructionsImg.GetHeight() / 2, true);
            settingsMenu = new GameObject(gc, settingsMenuImg, gc.GetGameWidth() / 2 - settingsMenuImg.GetWidth() / 2, 0, true);

            //Initializes the main text based UI elements of the game
            scoreText = new UITextObject(gc, 2, 2, Helper.YELLOW, true, "Score: " + score);
            healthText = new UITextObject(gc, 2, 1, Helper.RED, true, "HP: " + health);
            comboText = new UITextObject(gc, 0, 0, Helper.YELLOW, true, "Combo: " + scoreCombo);
            comboText.SetPosition(gc.GetUIWidth() - barWidth - comboText.GetWidth() - 2, 1);
            multiplierText = new UITextObject(gc, comboText.GetPos().x, 2, Helper.YELLOW, true, "Multiplier: " + scoreMultiplier);
            highScoreText = new UITextObject(gc, comboText.GetPos().x, 3, Helper.YELLOW, true, "High Score: " + highScore);
            obstacleSpawnText = new GameTextObject(gc, 1, 1, Helper.CYAN, true, "Next obstacle in: ");

            //Initializes the UI elements shown on the end game screen 
            backToMenuText = new UITextObject(gc, 0, 0, Helper.GREEN, true, "Press ENTER to return to the menu");
            backToMenuText.SetPosition(gc.GetUIWidth() / 2 - backToMenuText.GetWidth() / 2, 2);
            highCongratsText = new UITextObject(gc, 0, 0, Helper.YELLOW, true, "Congratulations. New High Score!!!");
            highCongratsText.SetPosition(gc.GetUIWidth() / 2 - highCongratsText.GetWidth() / 2, 3);

            //Initializes the score and health bar
            healthBar = new UIBarObject(gc, healthText.GetPos().x + healthText.GetWidth() + 3, 1, Helper.RED, true, MAX_HEALTH, health, barWidth);
            obstacleBar = new GameBarObject(gc, 1, 2, Helper.CYAN, true, (int)maxObsTime, (int)obsTimer, barWidth);
            comboBar = new UIBarObject(gc, gc.GetUIWidth() - barWidth - 1, 1, Helper.YELLOW, true, 5, scoreCombo, barWidth);

            //initializes the game over text
            gameOver = new GameObject(gc, gameOverImg, (gc.GetGameWidth() / 2) - (gameOverImg.GetWidth() / 2), 0, true);

            //sets the y position of the ground based obstacles to the ground and the flying obstacles to 2 pixels off the ground
            obsGroundPos = gc.GetGameHeight();
            obsFlyingPos = gc.GetGameHeight() - flyingAboveGround;
        }

        //Pre: the game container and a float representing the number of miliseconds per frame 
        //Post: None
        //Desc: calls subprograms to perform the logic the currently selected game state
        public override void Update(GameContainer gc, float deltaTime)
        {

            //This will exit your program with the x key.  You may remove this if you want       
            if (Input.IsKeyDown(ConsoleKey.X)) gc.Stop();

            switch (gameState)
            {
                case MENU:
                    //Get and implement menu interactions
                    UpdateMenu(gc);
                    break;

                case INSTRUCTIONS:
                    //Get user input to return to MENU
                    UpdateInstructions(gc);
                    break;

                case GAMEPLAY:
                    //Implement standared game logic (input, update game objects, apply physics, collision detection)
                    UpdateGamePlay(gc, deltaTime);
                    break;

                case ENDGAME:
                    //Wait for final input based on end of game options (end, restart, etc.)
                    UpdateEndGame(gc);
                    break;
            }
        }

        //Pre: the game container
        //Post: None
        //Desc: calls subprograms to draw objects and text on the screen based on what gamestate the game is currently in
        public override void Draw(GameContainer gc)
        {
            switch (gameState)
            {
                case MENU:
                    //Draw the possible menu options
                    DrawMenu(gc);
                    break;

                case INSTRUCTIONS:
                    //Draw the game instructions including prompt to return to MENU
                    DrawInstructions(gc);
                    break;

                case GAMEPLAY:
                    //Draw all game objects on each layers (background, middleground, foreground and user interface)
                    DrawGamePlay(gc);
                    break;

                case ENDGAME:
                    //Draw the final feedback and prompt for available options (exit,restart, etc.)
                    DrawEndGame(gc);
                    break;
            }
        }

        //Pre: game container
        //Post: None
        //Desc: Updates logic in the main menu       
        private void UpdateMenu(GameContainer gc)
        {
            //checks which key the user enters and moves the pointer or changes the game state accordingly
            if (Input.IsKeyDown(ConsoleKey.D))
            {
                //if the pointer is not at the last option move it one option to the right
                if (menuOption < NUM_OPTIONS)
                {
                    //increment the menu option and set the pointers position to the next menu option
                    menuOption++;
                    pointer.SetPosition(pointer.GetPos().x + pointerDiff, pointer.GetPos().y);
                }
            }
            else if (Input.IsKeyDown(ConsoleKey.A))
            {
                //if the pointer is not at the first option move it one option to the left
                if (menuOption > 1)
                {
                    //decrement the menu option and set the pointers position to the last menu option
                    menuOption--;
                    pointer.SetPosition(pointer.GetPos().x - pointerDiff, pointer.GetPos().y);
                }
            }
            else if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                //checks which menu option is selected and changes the game state
                switch (menuOption)
                {
                    case PLAY_OPT:
                        //changes the game state to the gameplay
                        gameState = GAMEPLAY;
                        break;

                    case INSTRUCTIONS_OPT:
                        //changes the game state to instructions
                        gameState = INSTRUCTIONS;
                        break;

                    case EXIT_OPT:
                        //ends the game 
                        gc.Stop();
                        break;
                }
            }
        }

        //Pre: game container
        //Post: None
        //Desc: Updates logic in the instructions menu
        private void UpdateInstructions(GameContainer gc)
        {
            //if the player hits escape return them to the menu
            if (Input.IsKeyDown(ConsoleKey.Escape))
            {
                //change the game state to manu
                gameState = MENU;
            }
        }

        //Pre: game container and a float delta time
        //Post: None
        //Desc: Updates logic when the game is running
        private void UpdateGamePlay(GameContainer gc, float deltaTime)
        {   
            //apply gravity effects
            ySpeed += PLAYER_GRAVITY;
            
            //if the low gravity buff is on redo some of the gravity effects
            if(buffEffects[LOW_GRAVITY_BUFF] == true)
            {
                //apply lowered gravity effects 
                ySpeed -= PLAYER_GRAVITY  / generalMultiplier;
            }
            
            //apply gravity to the projectile
            yProjSpeed += STONE_GRAVITY;

            //if the projectile is in the air change its timer
            if (stoneTimer > 0)
            {
                //change its timer's value 
                stoneTimer -= deltaTime;
            }

            //reduce the obstacle time by delta time
            obsTimer -= deltaTime;
            obstacleBar.SetValue((int)obsTimer);

            //checks for the current player state and changes it based on user input
            switch (playerState)
            {
                case STAND:
                    //checks if the user presses a key and changes the player state 
                    if (Input.IsKeyDown(ConsoleKey.S))
                    {
                        //changes player state to duck
                        playerState = DUCK;

                        //reset duck timer
                        duckTimer = MAX_DUCK_TIME;
                    }
                    else if (Input.IsKeyDown(ConsoleKey.W))
                    {
                        //changes player state to jump and start the jump
                        playerState = JUMP;
                        ySpeed = -jumpSpeed;
                    }
                    break;

                case DUCK:
                    //starts decrementing milliseconds to duck timer every frame
                    duckTimer -= (int)deltaTime;

                    //if the user presses W change the player state to jump
                    if (Input.IsKeyDown(ConsoleKey.W))
                    {
                        //change the player state to jump and start the jump
                        playerState = JUMP;
                        ySpeed = -jumpSpeed;

                        //reset the duck timer
                        duckTimer = MAX_DUCK_TIME;
                    }

                    //if the player has been ducking for 1.5 seconds change the player state back to stand
                    if (duckTimer <= 0)
                    {
                        //change the player state to stand
                        playerState = STAND;
                    }
                    break;

                case JUMP:
                    //Move the player up by its y speed
                    player[JUMP].Move(0, ySpeed);

                    //If the player is back on the ground change its y speed to 0 and go back to standing
                    if (CheckIfOnGround(gc, player[JUMP]) == true)
                    {
                        //set y speed to 0
                        ySpeed = 0;

                        //go back to standing
                        playerState = STAND;
                    }
                    break;
            }

            //if the user presses the space bar the character shoots the projectile
            if (Input.IsKeyDown(ConsoleKey.Spacebar))
            {
                //if there is no time on the projectile timer throw the stone
                if (stoneTimer <= 0)
                {
                    //set the stone's position to where the player is 
                    stone.SetPosition(player[playerState].GetPos().x + player[playerState].GetWidth(), player[playerState].GetPos().y);

                    //set the stones visibility to true
                    stone.SetVisibility(true);

                    //change the stone's speed
                    yProjSpeed = -throwSpeed;
                    xProjSpeed = 3 * throwSpeed;

                    //reset the projectile timer
                    stoneTimer = maxStoneTimer;
                }
            }

            //chacks if the projectile time is more then 0 move the stone
            if (stoneTimer > 0)
            {
                //move the stone
                stone.Move(xProjSpeed, yProjSpeed);

                //checks if stone touches the ground and stops it
                if (CheckIfOnGround(gc, stone) == true)
                {
                    //stop the stone
                    yProjSpeed = 0;
                    xProjSpeed = 0;
                }
            }
            else
            {
                //if the stone is visible make it invisible
                if (stone.GetVisibility() == true)
                {
                    //make the stone invisible
                    stone.SetVisibility(false);
                }
            }

            //checks if 1 second has passed see if its time to spawn an obstacle
            if (obsTimer <= 0)
            { 
                //generate a random number that will decide if an obstacle will be generated or if a buff might be generated (50% chance of spawning)
                randChanceNum = Helper.GetRandomNum(1, 101);               

                //the random number is 1 spawn an obstacle
                if (randChanceNum <= obsChance)
                {
                    //generate another random number to decide which obstacle will spawn out of the three
                    randChanceNum = Helper.GetRandomNum(0, 3);

                    //finds which obstacle the random number corresponds with and spawn it
                    switch (randChanceNum)
                    {
                        case SHORT_CHANCE:
                            //calls a subprogram to generate the obstacle
                            ObstacleGenerator(gc, shortObs, obsGroundPos);
                            break;

                        case TALL_CHANCE:
                            //calls a subprogram to generate the obstacle
                            ObstacleGenerator(gc, tallObs, obsGroundPos);
                            break;

                        case FLYING_CHANCE:
                            //calls a subprogram to generate the obstacle
                            ObstacleGenerator(gc, flyingObs, obsFlyingPos);
                            break;
                    }                
                }
                else if (activeBuff == false && randChanceNum >= buffChance)
                {                      
                        //set the buff's visibility to true as well as it sets its position to the end of the screen
                        buff.SetVisibility(true);
                        buff.SetPosition(gc.GetGameWidth(), gc.GetGameHeight() - buff.GetHeight());
                        activeBuff = true;                                                      
                }

                //resets the obstacle timer
                obsTimer = maxObsTime;
            }

            //call subprograms to move and collide the obstacles 
            ObstacleInteracter(gc, shortObs);
            ObstacleInteracter(gc, tallObs);
            ObstacleInteracter(gc, flyingObs);

            //call a subprogram to move and collide the buff
            BuffCollider(gc, deltaTime);

            //if the time to display the next level message is more than 0 decrement it
            if (nextLevelTime >= 0)
            {
                //decrement the time to display the next level message by delta time
                nextLevelTime -= deltaTime;
            }

            // if the user presses escape they are returned to a menu which acts as a pause menu as well
            if(Input.IsKeyDown(ConsoleKey.Escape))
            {
                //change the gamestate to menu
                gameState = MENU;
            }
        }

        //Pre: game container
        //Post: None
        //Desc: Updates logic when the game ended
        private void UpdateEndGame(GameContainer gc)
        {
            //if the user presses enter return them to the menu and reset the entire game
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                //calls a subprogram to reset the entire game 
                ResetGame(gc);

                //returns to the menu
                gameState = MENU;
            }
        }

        //Pre: game container
        //Post: None
        //Desc: Draws content in the main menu
        private void DrawMenu(GameContainer gc)
        {
            //Draw the interface elements of the menu
            gc.DrawToMidground(title);
            gc.DrawToMidground(playButton);
            gc.DrawToMidground(settingsButton);
            gc.DrawToMidground(exitButton);
            gc.DrawToMidground(pointer);
        }

        //Pre: game container
        //Post: None
        //Desc: Draws content in the instructions menu
        private void DrawInstructions(GameContainer gc)
        {
            //draws the image containing the game instructions
            gc.DrawToBackground(instructionsTxt);
            gc.DrawToBackground(settingsMenu);
        }

        //Pre: game container
        //Post: None
        //Desc: Draws content when the game is in progress
        private void DrawGamePlay(GameContainer gc)
        {
            //draw the player
            gc.DrawToMidground(player[playerState]);

            //draw the stone
            gc.DrawToBackground(stone);

            //calls a subprogram to draw the obstacles
            ObstacleDrawer(gc, shortObs);
            ObstacleDrawer(gc, tallObs);
            ObstacleDrawer(gc, flyingObs);

            //if the buff is currently visible draw it
            if(buff.GetVisibility())
            {
                //draw the buff 
                gc.DrawToForeground(buff);
            }

            //if the next level text is visible draw it
            if (nextLevelTime > 0)
            {
                //draw the next level text
                gc.DrawToBackground(nextLvlText);
            }

            //draw the current level indicators
            gc.DrawToBackground(currentLvlText);
            gc.DrawToBackground(levelBar);

            //draws the common user interface elements
            gc.DrawToUserInterface(scoreText);
            gc.DrawToUserInterface(healthText);
            gc.DrawToUserInterface(healthBar);
            gc.DrawToBackground(obstacleBar);
            gc.DrawToUserInterface(comboBar);
            gc.DrawToUserInterface(comboText);
            gc.DrawToUserInterface(multiplierText);
            gc.DrawToUserInterface(highScoreText);
            gc.DrawToBackground(obstacleSpawnText);

            //if there was a collision between the player and the buff image draw the UI            
            if (buffCollision == true)
            {                
                //draws the text related to which buff is currently active
                gc.DrawToBackground(buffText);

                //draws the buff bar only if its not for the health buff
                if (randomBuff <= LOW_GRAVITY_BUFF)
                {                 
                    //draws the buff bar
                    gc.DrawToBackground(buffBar);
                }                
            }
        }

        //Pre: game container
        //Post: None
        //Desc: Draws content when the game ended
        private void DrawEndGame(GameContainer gc)
        {
            //draw the dead player
            gc.DrawToMidground(player[DEAD]);

            //draw the UI elements
            gc.DrawToUserInterface(healthText);
            gc.DrawToUserInterface(scoreText);
            gc.DrawToUserInterface(endLevelText);

            //draws the game over message and the return to the menu message
            gc.DrawToBackground(gameOver);
            gc.DrawToUserInterface(backToMenuText);

            //if there is a high score congratulate the player
            if (isScoreHigh == true)
            {
                //congratulates the player
                gc.DrawToUserInterface(highCongratsText);
            }
        }

        //Pre: the game object the player wants to check for and the game container
        //Post: true or false whether the player is on the ground or not
        //Desc: checks if a game object is on the ground 
        private bool CheckIfOnGround(GameContainer gc, GameObject thing)
        {
            //checks if the object is on the ground and returns true if it is
            if (thing.GetPos().y + thing.GetHeight() == gc.GetGameHeight())
            {
                //rteturn true
                return true;
            }
            //return false
            return false;
        }

        //Pre: an array containing one of the three obstacles and the game container
        //Post: None
        //Desc: draws the obstacles that are currently visible
        private void ObstacleDrawer(GameContainer gc, GameObject[] obstacle)
        {
            //loops through all the obstacles in the array and draws the ones that are visible
            for (int i = 0; i < obstacle.Length; i++)
            {
                //checks if the obstacle is visible
                if (obstacle[i].GetVisibility() == true)
                {
                    //draws the obstacle
                    gc.DrawToForeground(obstacle[i]);
                }
            }
        }

        //Pre: an array of obstacles, and image containing the obstacles appearancy and the game container
        //Post: None
        //Desc: Initializes the obstacles
        private void ObstacleLoader(GameContainer gc, GameObject[] obstacle, Image obstacleImg)
        {
            //generates every short obstacle in the array
            for (int i = 0; i < obstacle.Length; i++)
            {
                //initializes the selected obstacle
                obstacle[i] = new GameObject(gc, obstacleImg, 1, 1, false);
            }
        }

        //Pre: an array containing one of the three obstacles and the game container
        //Post: None
        //Desc: generates obstacles 
        private void ObstacleGenerator(GameContainer gc, GameObject[] obstacle, int yPos)
        {
            //loops through and checks if there is an available obstacle in the entire array and generates it
            for (int i = 0; i < obstacle.Length; i++)
            {
                //if the obstacle is currently invisible makes it visible
                if (obstacle[i].GetVisibility() == false)
                {
                    //changes its location to the end of the screen
                    obstacle[i].SetPosition(gc.GetGameWidth(), yPos - obstacle[i].GetHeight());
                  
                    //makes the obstacle visible
                    obstacle[i].SetVisibility(true);
                    break;
                }
            }
        }

        //Pre: an array containing one of the three obstacles and the game container
        //Post: None
        //Desc: Moves the obstacles, checks if they reached the end of the console and calls subprograms for collision detection and the effects of collisions 
        private void ObstacleInteracter(GameContainer gc, GameObject[] obstacle)
        {
            //loops through every obstacle in an array to see which one is active and interacts them
            for (int i = 0; i < obstacle.Length; i++)
            {
                //checks if the obstacle is visible and moves it
                if (obstacle[i].GetVisibility() == true)
                {
                    //moves the obstacle
                    obstacle[i].Move(-obsSpeed, 0);

                    //if it reaches the end of the screen despawn it
                    if (obstacle[i].GetPos().x == 0)
                    {
                        //makes the obstacle invisible
                        obstacle[i].SetVisibility(false);

                        //calls a subprogram to increment the score
                        ScoreAdder(gc);
                    }

                    //checks if any of the obstacles are colliding with the current player and calculate the effects                 
                    if (Helper.FastIntersects(obstacle[i], player[playerState]) && buffEffects[INVINCIBLE_BUFF] == false)
                    {
                        //set the obstacles visibility to false
                        obstacle[i].SetVisibility(false);

                        //decrement the health and update health UI
                        health -= obstacleDamage;
                        UpdateHealth();

                        //if the low gravity buff is active turns it off
                        if (buffEffects[LOW_GRAVITY_BUFF] == true)
                        {
                            //turn off the low gravity buff and reset the buffs in general
                            buffEffects[LOW_GRAVITY_BUFF] = false;
                            buffCollision = false;
                            activeBuff = false;
                        }

                        //if the health is less than 0 end the game
                        if (health <= 0)
                        {
                            //set the gamestate to endgame
                            gameState = ENDGAME;

                            //updates the text that shows what level the player reached
                            endLevelText.UpdateText("You have reached level " + levelCount);
                        }

                        //change the combos and multipliers and update the UI
                        scoreCombo = 0;
                        scoreMultiplier = 1;
                        comboBar.SetValue(scoreCombo);
                        comboText.UpdateText("Combo: " + scoreCombo);
                        multiplierText.UpdateText("Multiplier: " + scoreMultiplier);
                    }

                    //if plane shift buff is active let all obsacles collide with the projectile otherwise only let the tall obstacle
                    if(buffEffects[PLANE_SHIFT_BUFF] == true)
                    {
                        //calls a subprogram to collide the obstacle with the projectile
                        ObstacleProjectileColl(gc, obstacle[i]);
                    }
                    else if(obstacle[i] == tallObs[i])
                    {
                        //calls a subprogram to collide the obstacle with the projectile
                        ObstacleProjectileColl(gc, obstacle[i]);
                    }
                }
            }
        }

        //Pre: The game container and the current obstacle that is being moved and interacted
        //Post: None
        //Desc: Collides and calculates the effects of collision between obstacles and the projectile
        private void ObstacleProjectileColl(GameContainer gc, GameObject obstacle)
        {
            //checks if the tall obstacle collides with the visible stone
            if (Helper.FastIntersects(obstacle, stone) && stone.GetVisibility() == true)
            {
                //calls a subprogram to increment the score and other score variables
                ScoreAdder(gc);

                //set the visibility of the colliding objects to false
                stone.SetVisibility(false);
                obstacle.SetVisibility(false);

                //reset the projectile's timer
                stoneTimer = 0;
            }
        }

        //Pre: the game container
        //Post: None
        //Desc: Moves and collides the buffs/debuffs as well as it calculates the effects of the collisions
        private void BuffCollider(GameContainer gc, float deltaTime)
        {
            if(buff.GetVisibility())
            {
                //moves the buff towards the player
                buff.Move(-obsSpeed, 0);

                //if the buff collides with the player calculates the effects
                if (Helper.FastIntersects(player[playerState], buff))
                {
                    //gets a random number that will decide which buff effect is added to the player
                    randomBuff = Helper.GetRandomNum(0, NUM_BUFF);                   

                    //checks what kind of buff is currently selected
                    switch (randomBuff)
                    {
                        case HEALTH_BUFF:
                            //if the players health is not max adds to it
                            if (health != MAX_HEALTH)
                            {
                                //adds one to the health
                                health += 1;
                                UpdateHealth();
                            }
                            break;

                        case MINUS_HEALTH_BUFF:
                            //sets the buff display time to max
                            buffDisTime = maxBuffTime;

                            //if the players health is not at minimum takes away from it
                            if (health != 1)
                            {
                                //takes one away from health
                                health -= 1;
                                UpdateHealth();
                            }   
                            break;

                        case COMBO_KILLER_BUFF:
                            //resets the combo and the multiplier and updates the UI
                            scoreCombo = 0;
                            scoreMultiplier = 1;
                            comboBar.SetValue(scoreCombo);
                            comboText.UpdateText("Combo: " + scoreCombo);
                            multiplierText.UpdateText("Multiplier: " + scoreMultiplier);
                            break;
                    }

                    //if the random buff is the gravity one keep it on for longer
                    if(randomBuff == LOW_GRAVITY_BUFF)
                    {
                        //sets the buff diplay time to 3x the normal
                        buffDisTime = maxBuffTime * generalMultiplier;
                    }
                    else
                    {
                        //sets the buff display time to max
                        buffDisTime = maxBuffTime;
                    }                    

                    //sets the buff collision and the invincibility variables to true
                    buffCollision = true;
                    buffEffects[randomBuff] = true;

                    //makes the buff invisible
                    buff.SetVisibility(false);
                }
                else if (buff.GetPos().x == 0)
                {
                    //if the buff hits the end of the map makes it invisible and turns
                    buff.SetVisibility(false);

                    //sets active buff to false
                    activeBuff = false;
                }
            }
            
            //if a collision has happened calculates its effects
            if(buffCollision == true)
            {
                //substracts time from buff dislpay time every frame and update the buff bar accordingly            
                buffDisTime -= deltaTime;
                
                //if the random buff is the gravity one alter the displayed time left
                if(randomBuff == LOW_GRAVITY_BUFF)
                {
                    //makes the displayed buff time left elapse slower
                    buffBar.SetValue((int)buffDisTime / generalMultiplier);
                }
                else
                {
                    //updates the buff UI
                    buffBar.SetValue((int)buffDisTime);
                }
                
                //if the buff display time reached 0 stop the effects of the buff
                if (buffDisTime <= 0)
                {
                    //set players no score, the buff's activeness and its collision to false
                    buffEffects[randomBuff] = false;
                    activeBuff = false;
                    buffCollision = false;
                }

                //check which buff text has to be displayed and displays it
                switch (randomBuff)
                {
                    case HEALTH_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("+ Health");
                        break;

                    case MINUS_HEALTH_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("- Health");
                        break;

                    case INVINCIBLE_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("Invincible");
                        break;

                    case NO_SCORE_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("No Score");
                        break;

                    case PLANE_SHIFT_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("Plane Shift");
                        break;

                    case COMBO_KILLER_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("Combo Killed");
                        break;

                    case LOW_GRAVITY_BUFF:
                        //set the buff text to the approporiate message
                        buffText.UpdateText("Low Gravity");
                        break;
                }

                //center the buff text and draw it onto the screen
                buffText.SetPosition(gc.GetGameWidth() / 2 - buffText.GetWidth() / 2, buffText.GetPos().y);
            }                     
        }

        //Pre: the game container and the an array of game objects containing the obstacles
        //Post: None
        //Desc: resets all visible obstacles to become invisible
        private void ObstacleResetter(GameContainer gc, GameObject[] obstacle)
        {
            //loops through every obstacle in an array and makes the visible ones invisible
            for (int i = 0; i < obstacle.Length; i++)
            {
                //if the obstacle is visible make it invisible
                if(obstacle[i].GetVisibility())
                {
                    //make the obstacle invisible
                    obstacle[i].ToggleVisibility();
                }
            }
        }

        //Pre: the game container
        //Post: None
        //Desc: Resets the map to get ready for a new level or even a restart of the entire game (does not reset health or score because of the levels)
        private void ResetLevel(GameContainer gc)
        {
            //increment the level count and update the displayed text
            levelCount += 1;
            currentLvlText.UpdateText("Current level: " + levelCount);

            //reset game variables to their initial values (off values)
            nextLevelTime = maxLevelTime;
            scoreCombo = 0;
            scoreMultiplier = 1;
            obsDestroyed = 0;
            obsTimer = maxObsTime;
            obsChance = FIRST_OBS_CHANCE;

            //if a buff is currently in effect make sure the end its effects otherwise make sure to make all moving buffs invisible 
            if(buffCollision == true)
            {                
                //resets the buff display time
                buffDisTime = 0;

                //if the low gravity buff is active deactivates it
                if (buffEffects[LOW_GRAVITY_BUFF] == true)
                {
                    //deactivates the low gravity buff
                    buffEffects[LOW_GRAVITY_BUFF] = false;
                    buffCollision = false;
                    activeBuff = false;
                }
            }
            else if(activeBuff == true)
            {
                //resets the buff image
                buff.SetVisibility(false);
                activeBuff = false;
            }

            //updates the UI elements
            comboBar.SetValue(scoreCombo);          
            comboText.UpdateText("Combo: " + scoreCombo);
            multiplierText.UpdateText("Multiplier: " + scoreMultiplier);
            levelBar.SetValue(obsDestroyed);
            levelBar.SetMax(maxObsDestroyed * levelCount);

            //if the maximum obstacle spawn time is not yet at its minimum decrement it
            if (maxObsTime > minObsTime)
            {
                //decrement max obstacle spawn time by 0.05 seconds
                maxObsTime -= obsTimeDecrement;
            }

            //if the obstacle spawn chance isnt at its maximum increment it
            if (obsChance > maxObsChance)
            {
                //increment the obstacle spawn chance
                obsChance += obsChancePlus;
            }           

            //checks which player state the player is in and if its not stand makes them stand
            switch (playerState)
            {
                case JUMP:
                    //moves the jumping player to the ground and sets them to stand
                    player[JUMP].SetPosition(player[JUMP].GetPos().x, gc.GetGameHeight() - player[JUMP].GetHeight());
                    playerState = STAND;
                    break;

                case DUCK:
                    //reset the duck timer and makes the player stand
                    duckTimer = 0;
                    playerState = STAND;
                    break;
            }

            //calls a subprogram to reset the obstacles
            ObstacleResetter(gc, shortObs);
            ObstacleResetter(gc, tallObs);
            ObstacleResetter(gc, flyingObs);

            //if the stone is visible make it invisible
            if (stone.GetVisibility())
            {
                //makes the stone invisible
                stone.SetVisibility(false);
            }
        }

        //Pre: The game container
        //Post: None
        //Desc: Uses the Reset Level method to completely reset all game variables
        private void ResetGame(GameContainer gc)
        {
            //calls a subprogram to reset the level
            ResetLevel(gc);

            //resets remaining game variables
            maxObsTime = FIRST_OBS_TIME;
            levelCount = 1;
            health = MAX_HEALTH;
            score = 0;
            nextLevelTime = 0;
            isScoreHigh = false;

            //updates remaining UI elements
            UpdateHealth();
            scoreText.UpdateText("Score: " + score);
            currentLvlText.UpdateText("Current level: " + levelCount);
            levelBar.SetMax(maxObsDestroyed);
        }

        //Pre: None
        //Post: None
        //Desc: Increments the score well as all the other score variables and it checks for combos and high scores
        private void ScoreAdder(GameContainer gc)
        {
            //if the no score debuff is currently not active increment the player's score
            if (buffEffects[NO_SCORE_BUFF] == false)
            {
                //increment the score and update UI
                score += scoreIncrementer * scoreMultiplier;
                scoreCombo += 1;

                //if the player has reached a combo of 5 reset the counter and add a multiplier
                if (scoreCombo == 5)
                {
                    //change the score conditions
                    scoreCombo = 0;
                    scoreMultiplier += 0.5f;
                    multiplierText.UpdateText("Multiplier: " + scoreMultiplier);
                }

                //if the current score is larger than the high score turn on the is score high variable
                if (score > highScore && isScoreHigh == false)
                {
                    //makes the is score high variable true
                    isScoreHigh = true;
                }

                //if the score is the current high score make the current score the high score
                if (isScoreHigh == true)
                {
                    //update high score with the current score
                    highScore = score;
                    highScoreText.UpdateText("High Score: " + highScore);
                }

                //increment the number of obstacles destroyed
                obsDestroyed += 1;

                //if the player destroyed 5 obstacles move them to the next level otherwise just updates the rest of the socre UI 
                if (obsDestroyed == (maxObsDestroyed * levelCount))
                {                   
                    //call a subprogram to reset the map
                    ResetLevel(gc);                    
                }
                else
                {
                    //updates score UI
                    scoreText.UpdateText("Score: " + score);
                    comboText.UpdateText("Combo: " + scoreCombo);
                    comboBar.SetValue(scoreCombo);
                    levelBar.SetValue(obsDestroyed);
                }        
            }
        }

        //Pre: None
        //Post: None
        //Desc: Updates health UI and it resets the gravity buff as that is directly related to losing health
        private void UpdateHealth()
        {
            //Updates the health text and health bar
            healthBar.SetValue(health);
            healthText.UpdateText("HP: " + health);           
        }        
    }
}
