﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LePuissance4ParAntoineEtLea
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Puissance4 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        private SpriteFont textFont;
        SpriteBatch spriteBatch;
        private byte[,] damier;  //stocke l'emplacement des pions
        private ObjetPuissance4 cadre, pionJaune, pionRouge,touchesClavier;
        private const int VX = 7;
        private const int VY = 6;
        private int colonnePionAPlacer;  //la colonne selectionnée pour poser un pion
        private byte joueurActuel;  //1=Joueur Jaune;  2=Joueur Rouge;
        private byte gagnant; //joueur ayant gagné, au moment ou la partie se finit
        private bool partieEnCours; //est vrai si une partie est en cours
        private bool botActif; //est vrai si le joueur affronte un bot
        private bool menuActif; //est vrai si le menu doit etre affiché
        private Bot botJeu;  
        private bool tourBot; //est vrai si c'est au bot de jouer
        private Texture2D background;      // les images de fond du jeu
        private Texture2D backgroundMenu;  // idem
        private Vector2 backgroundPos = Vector2.Zero;
        private SoundEffect effect; // un son de fin de jeu 
                
        private Bouton[] tabBoutons;  //stocke les differents boutons

        // on stocke les etats précedent du clavier et de la souris afin d'eviter que les elements 
        // interactifs soient activés plusieurs fois par clics
        private KeyboardState oldState;  // stocke l'etat du clavier de la frame précedente
        private MouseState oldMouseState; // stocke l'etat de la souris de la frame précedente
        

        public Puissance4()
        {
            Window.Title = "Monster 4";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            damier = new byte[VY, VX]{
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 }
            };
            colonnePionAPlacer = 0;
            joueurActuel = 1;
            partieEnCours = false;
            gagnant = 0;
            botActif = false;
            botJeu = new Bot();
            tourBot = false;
            menuActif = true;

            int nbBoutons = 6;
            tabBoutons = new Bouton[nbBoutons];

            //fonctionDeTest();  //decommenter pour que les tests soient effetués au lancement du jeu

            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // résolution du jeu
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 920;
            graphics.ApplyChanges();

            this.textFont = Content.Load<SpriteFont>("MyFont");

            ////sprites
            cadre = new ObjetPuissance4(Content.Load<Texture2D>("images\\cadre"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionJaune = new ObjetPuissance4(Content.Load<Texture2D>("images\\jaune"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionRouge = new ObjetPuissance4(Content.Load<Texture2D>("images\\rouge"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            touchesClavier= new ObjetPuissance4(Content.Load<Texture2D>("images\\touchesClavier"), new Vector2(50, 600), new Vector2(534f, 203f));

            //background
            background = Content.Load<Texture2D>("images\\fondecran");
            backgroundMenu = Content.Load<Texture2D>("images\\menu");


            //sons
            effect = Content.Load<SoundEffect>("sons\\gagne");


            ////boutons
            //mise en place d'une texture de couleur unie
            int xText = 400, yText = 90;
            Texture2D texture_btn_400x90 = new Texture2D(GraphicsDevice, xText, yText);
            Color[] tabColor = new Color[xText * yText];
            for (int i = 0; i < xText * yText; ++i)
            {
                tabColor[i] = Color.CadetBlue;
            }
            texture_btn_400x90.SetData(tabColor, 0, xText * yText);


            // boutons de jeu
            tabBoutons[0] = new Bouton("Rejouer", false, texture_btn_400x90, new Vector2((1024 / 4) - xText / 2, 920 - yText - 50), new Vector2(400f, 90f));
            tabBoutons[1] = new Bouton("Retour Menu", false, texture_btn_400x90, new Vector2((1024 * 3 / 4 - xText / 2), 920 - yText - 50), new Vector2(400f, 90f));

            // boutons du menu
            int nbBoutonsMenu = 4;
            int yOffset = (920 - nbBoutonsMenu * yText) / (nbBoutonsMenu + 1);
            int yPos = yOffset;
            int xPos = (1024 * 3 / 4 - xText / 2);
            tabBoutons[2] = new Bouton("Jouer entre humains", true, texture_btn_400x90, new Vector2(xPos, yPos), new Vector2(400f, 90f));
            yPos += yOffset + yText;
            tabBoutons[3] = new Bouton("Jouer contre le bot facile", true, texture_btn_400x90, new Vector2(xPos, yPos), new Vector2(400f, 90f));
            yPos += yOffset + yText;
            tabBoutons[4] = new Bouton("Jouer contre le bot intermediaire", true, texture_btn_400x90, new Vector2(xPos, yPos), new Vector2(400f, 90f));
            yPos += yOffset + yText;
            tabBoutons[5] = new Bouton("Jouer contre le bot difficile", true, texture_btn_400x90, new Vector2(xPos, yPos), new Vector2(400f, 90f));
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //note: l'attribut oldState permet d'ignorer la touche pressée si le joeur reste appuyé dessus
            // c'est nécessaire car sinon chaque commande est appelée plusieurs fois a chaques pression du bouton

            ///////// entrées clavier
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
            {
                if (colonnePionAPlacer < VX - 1) colonnePionAPlacer++;

            }
            if (keyboard.IsKeyDown(Keys.Left) && !oldState.IsKeyDown(Keys.Left))
            {
                if (colonnePionAPlacer > 0) colonnePionAPlacer--;
            }

            if (partieEnCours)
            {
                if (botActif && tourBot)
                {
                    int choixBot = botJeu.choixColonne(damier);
                    bool succesPose=posePion(choixBot);
                    if(succesPose) tourBot = false;
                }
                else
                {
                    if ((keyboard.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space))
                         || (keyboard.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down)))
                    {
                        bool succesPose=posePion(colonnePionAPlacer);
                        if (succesPose &&botActif && partieEnCours) tourBot = true;
                    }
                }

                if (testDamierRemplit(damier))
                {
                    partieEnCours = false;
                    gagnant = 0;
                }
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
                {
                    nouvellePartie();

                }
            }

            
            oldState = keyboard;


            //// interactions souris
            bool etatMenu = menuActif; // on stocke l'etat actuel du menu car il peut changer durant le foreach
            foreach (Bouton btn in tabBoutons)
            {
                // on détermine si le bouton est visible selon menuActif et la proprieté menu du bouton
                bool visible = etatMenu ? btn.Menu : !btn.Menu;
                if (btn.isOver(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)) && visible)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouseState.LeftButton !=ButtonState.Pressed  )
                    {
                        //il faut prévoir l'action a effectuer pour chaque bouton
                        switch (btn.Texte)
                        {
                            case "Rejouer":
                                nouvellePartie();
                                break;
                            case "Retour Menu":
                                menuActif = true;
                                break;
                            case "Jouer entre humains":
                                botActif = false;
                                menuActif = false;
                                nouvellePartie();
                                break;
                            case "Jouer contre le bot facile":
                                botActif = true;
                                tourBot = false;
                                menuActif = false;
                                botJeu.SetDifficulte(1);
                                nouvellePartie();
                                break;
                            case "Jouer contre le bot intermediaire":
                                botActif = true;
                                tourBot = false;
                                menuActif = false;
                                botJeu.SetDifficulte(2);
                                nouvellePartie();
                                break;
                                
                            case "Jouer contre le bot difficile":
                                botActif = true;
                                tourBot = false;
                                menuActif = false;
                                botJeu.SetDifficulte(3);
                                nouvellePartie();
                                break;
                                
                            default:
                                break;
                        }
                    }
                }
            }
            oldMouseState = Mouse.GetState();
        }

        /// <summary>
        /// renitialise le plateau.
        /// renitialise les params partieEnCours et gagnant si la partie etait finie
        /// </summary>
        private void nouvellePartie()
        {
            //le plateau est renitialisé
            damier = new byte[VY, VX]{
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 }
                        };
            //si la partie etait bien finie
            if (!partieEnCours)
            {
                partieEnCours = true;
                if (gagnant != 0)
                {
                    joueurActuel = gagnant;
                    gagnant = 0;
                }
            }
            
            //si la partie n'etait pas finie, on se contente de renitialiser le plateau
            
        }

        /// <summary>
        /// met a jour la grille pour placer un pion du joueur actuel  
        /// renvoit faux si la colonne n'est pas valide (si elle est remplie)
        /// </summary>
        /// <param name="colonne"></param>
        /// <returns></returns>
        private bool posePion(int colonne)
        {
            if (damier[0, colonne] == 0)
            {
                int y = 0;
                while (y < VY && damier[y, colonne] == 0)
                {
                    y++;
                }
                damier[y - 1, colonne] = joueurActuel;
                if (testFin(joueurActuel, colonne, y - 1))
                {
                    gagnant = joueurActuel;
                    partieEnCours = false;

                    // effet sonore de fin de partie
                    effect.Play();
                }


                joueurActuel = (byte)((joueurActuel == (byte)1) ? 2 : 1);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PapayaWhip);

            spriteBatch.Begin();

            int offsetX = 140;
            int offsetY = 100;

            //// dessin grille de jeu
            if (!menuActif)
            {

                //background
                spriteBatch.Draw(background, backgroundPos, Color.White);

                //damier
                for (int x = 0; x < VX; x++)
                {
                    for (int y = 0; y < VY; y++)
                    {
                        int xpos, ypos;
                        xpos = offsetX + x * 100;
                        ypos = offsetY + y * 100;
                        Vector2 pos = new Vector2(xpos, ypos);

                        if (damier[y, x] == 1)
                        {
                            spriteBatch.Draw(pionJaune.Texture, pos, Color.White);
                        }
                        if (damier[y, x] == 2)
                        {
                            spriteBatch.Draw(pionRouge.Texture, pos, Color.White);
                        }
                        spriteBatch.Draw(cadre.Texture, pos, Color.White);

                    }
                }

                //// dessin divers
                //// traitement selon l'etat de la partie
                if (partieEnCours)
                {
                    //on dessine un pion au dessus de la colonne sélectionnée
                    Vector2 posAPlacer = new Vector2(offsetX + colonnePionAPlacer * 100, 0);
                    ObjetPuissance4 pion = (joueurActuel == 1 ? pionJaune : pionRouge);
                    spriteBatch.Draw(pion.Texture, posAPlacer, Color.White);
                }
                else
                {
                    string messageFin = string.Format("C'est fini! Le joueur " + (gagnant == 1 ? "vert" : "rouge") + " est le gagnant! Appuyez sur Enter pour rejouer");
                    Vector2 position = new Vector2(100, 20);
                    spriteBatch.DrawString(this.textFont, messageFin, position, Color.White);

                }
            }
            else  // dans le menu principal
            {
                //background
                spriteBatch.Draw(backgroundMenu, backgroundPos, Color.White);

                //fleches
                spriteBatch.Draw(touchesClavier.Texture, touchesClavier.Position, Color.White);
            }

            

            ////boutons
            foreach (Bouton btn in tabBoutons)
            {

                btn.draw(spriteBatch, textFont,menuActif);

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// test si le joueur donné en paramètre gagne en posant son pion aux coordonnées xIn, yIN  (la fonction ne verifie pas l'etat du damier a ces coordonnées (si il est occupé et par quel joueur )
        /// </summary>
        /// <param name="joueur"></param>
        /// <param name="xIN"></param>
        /// <param name="yIN"></param>
        /// <returns></returns>
        private bool testFin(byte joueur, int xIN, int yIN)
        {
            bool aGagne = false;
            if (xIN >= 0 && xIN < VX && yIN >= 0 && yIN < VY)
            {
                //a ce point la, on a damier[yIN,xIn] qui renvoit la position du pion placé a la colonne donnée
                //On doit tester si il y a 4 pions du joueur dans les 4 directions autours du pion placé
                int xOffset = 0;
                int yOffset = 1;
                int nbPions = comptePionsDirection(xIN, yIN, xOffset, yOffset, joueur,damier);
                xOffset = 1;
                for (yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int nbPionsTemp = comptePionsDirection(xIN, yIN, xOffset, yOffset, joueur,damier);
                    nbPions = (nbPions > nbPionsTemp ? nbPions : nbPionsTemp); //on prend le max de toutes les directions possibles
                }



                if (nbPions >= 4)
                {
                    aGagne = true;
                    Console.WriteLine("detection win...joueur " + joueur + ", colonne " + xIN + ".");
                }
            }
           
            return aGagne;
        }


        /// <summary>
        /// compte les pions d'un meme joueur autours d'une position donnée et selon une direction donnée par xOffset et yOffset
        /// </summary>
        /// <param name="xIN"></param>
        /// <param name="yIN"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="joueur"></param>
        /// <param name="damier"></param>
        /// <returns></returns>
        private int comptePionsDirection(int xIN, int yIN, int xOffset, int yOffset, byte joueur, byte[,] damier)
        {
            int nbPions = 1; //commence a 1 car le pion posé est compté d'office
            for (int sens = -1; sens <= 1; sens += 2)
            {
                int x = xIN + xOffset;
                int y = yIN + yOffset;
                while (x >= 0 && x < VX && y >= 0 && y < VY && damier[y, x] == joueur)
                {
                    nbPions++;
                    x += xOffset;
                    y += yOffset;
                }
                xOffset *= -1;
                yOffset *= -1;
            }

            return nbPions;
        }

        /// <summary>
        ///renvoit vrai si le damier est remplit 
        ///
        /// </summary>
        /// <param name="damier"></param>
        /// <returns></returns>
        public static bool testDamierRemplit(byte[,] damierInput)
        {
            foreach(byte b in damierInput)
            {
                if (b == 0) return false;
            }
            return true;
        }
        


        /// <summary>
        /// fonction servant a faire des tests rapides.
        /// </summary>
        public void fonctionDeTest()
        {
            damier = new byte[VY, VX]{
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 2, 0, 0, 0 },
                {2, 0, 0, 2, 1, 0, 0 },
                {1, 0, 2, 1, 1, 0, 0 }
            };

            damierMiniMax damiertest = new damierMiniMax(damier);
            Console.WriteLine(damiertest.Damier.ToString());
            byte[,] damiertestnew = new byte[VY, VX];

            damierMiniMax damierMM = new damierMiniMax(damier);
            damierMM.GetSuccesseurs(1);
            
            for (int colonne = 0; colonne < VX; colonne++)
            {
                for(int y = 0; y < VY; y++)
                {
                    Console.WriteLine("(" + y + " , " + colonne + " ) =>" + damiertest.Damier[y, colonne]);
                }
            }

            Console.WriteLine(VY+"  damier.GetLength(0)="+damier.GetLength(0));
            Console.WriteLine(VX+"  damier.GetLength(1)=" + damier.GetLength(1));
            
        }
    }
}
