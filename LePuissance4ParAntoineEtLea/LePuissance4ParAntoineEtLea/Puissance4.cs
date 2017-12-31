using System;
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
        private byte[,] damier;
        private ObjetPuissance4 cadre, pionJaune, pionRouge;
        private const int VX = 7;
        private const int VY = 6;
        private int colonnePionAPlacer;  //la colonne selectionnée pour poser un pion
        private byte joueurActuel;  //1=Joueur Jaune;  2=Joueur Rouge;
        private byte gagnant; //joueur ayant gagné, au moment ou la partie se finit
        private bool partieEnCours; //est vrai si une partie est en cours
        private bool botActif; //est vrai si le joueur affronte un bot
        private Bot botJeu;
        private bool tourBot; //est vrai si c'est au bot de jouer

        private KeyboardState oldState;  // stocke l'etat du clavier de la frame précedente


        public Puissance4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            partieEnCours = true;
            gagnant = 0;
            botActif = true;
            botJeu = new Bot();
            tourBot = false;
            //fonctionDeTest();
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

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 920;
            graphics.ApplyChanges();

            this.textFont = Content.Load<SpriteFont>("MyFont");

            cadre = new ObjetPuissance4(Content.Load<Texture2D>("images\\cadre"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionJaune = new ObjetPuissance4(Content.Load<Texture2D>("images\\jaune"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionRouge = new ObjetPuissance4(Content.Load<Texture2D>("images\\rouge"), new Vector2(0f, 0f), new Vector2(100f, 100f));
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
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
                {
                    damier = new byte[VY, VX]{
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 },
                            {0, 0, 0, 0, 0, 0, 0 }
                        };
                    joueurActuel = gagnant;
                    partieEnCours = true;
                    gagnant = 0;

                }
            }

            oldState = keyboard;
            base.Update(gameTime);
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
            // TODO: Add your drawing code here
            int offsetX = 140;
            int offsetY = 100;

            if (partieEnCours)
            {
                //on dessine un pion au dessus de la colonne sélectionnée
                Vector2 posAPlacer = new Vector2(offsetX + colonnePionAPlacer * 100, 0);
                ObjetPuissance4 pion = (joueurActuel == 1 ? pionJaune : pionRouge);
                spriteBatch.Draw(pion.Texture, posAPlacer, Color.White);
            }
            else
            {
                string messageFin = string.Format("C'est fini! Le joueur " + (gagnant == 1 ? "jaune" : "rouge") + " est le gagnant! \n  appuyez sur enter pour rejouer");
                Vector2 position = new Vector2(100, 10);
                spriteBatch.DrawString(this.textFont, messageFin, position, Color.Black);
            }

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



        public void fonctionDeTest()
        {
            damier = new byte[VY, VX]{
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 2, 2, 0, 2 },
                {1, 1, 0, 1, 1, 1, 1 }
            };

            for (int colonne = 0; colonne < VX; colonne++)
            {
            }

            Console.WriteLine(VY+"  damier.GetLength(0)="+damier.GetLength(0));
            Console.WriteLine(VX+"  damier.GetLength(1)=" + damier.GetLength(1));
        }
    }
}
