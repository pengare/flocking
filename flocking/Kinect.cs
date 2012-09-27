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
using Microsoft.Kinect;
//using Microsoft.Speech.AudioFormat;
//using Microsoft.Speech.Recognition;


namespace flocking
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Kinect : Microsoft.Xna.Framework.GameComponent
    {
        private Game1 game;
        private Camera camera;
        //private Player player;


        KinectSensor kinectSensor;
        private Skeleton[] skeletonData;
        //private Skeleton skeleton;
        private int skeletonId = -1;

        //KinectAudioSource source;
        //SpeechRecognitionEngine sre;


        #region User Position
        //this roation will send to camera and cause update
        private float YRotation = 0.0f;
        private float XRotation = 0.0f;

        private Vector3 prevRightHandPos;
        private Vector3 currRightHandPos;

        private Vector3 prevLeftHandPos;
        private Vector3 currLeftHandPos;

        private float prevRightHandYAngle;
        private float currRightHandYAngle;

        private float prevRightHandXAngle;
        private float currRightHandXAngle;

        private float prevLeftHandYAngle;
        private float currLeftHandYAngle;

        private float prevLeftHandXAngle;
        private float currLeftHandXAngle;


        //This is for shoot gesture
        private bool bInShoot = false;
        private float shootStep = 0.0f;
        private float shootThreshold = 5.0f;
        #endregion

        public Kinect(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            //initialize Kinect SDK
            LoadGameComponents();
            kinectSensor = (from sensorToCheck in KinectSensor.KinectSensors
                            where sensorToCheck.Status == KinectStatus.Connected
                            select sensorToCheck).FirstOrDefault();

            kinectSensor.DepthStream.Enable();
            TransformSmoothParameters param = new TransformSmoothParameters();
            param.Smoothing = 0.2f;
            param.Correction = 0.0f;
            param.Prediction = 0.0f;
            param.JitterRadius = 0.2f;
            param.MaxDeviationRadius = 0.3f;

            kinectSensor.SkeletonStream.Enable(param);
            kinectSensor.Start();

            kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinectSensor_AllFrameReady);

            //setup speech recognition
           // SetupSpeech();

            base.Initialize();
        }

        //private void SetupSpeech()
        //{
        //    //setup audio
        //    source = kinectSensor.AudioSource;//new KinectAudioSource();
        //    source.EchoCancellationMode = EchoCancellationMode.None;
        //    source.AutomaticGainControlEnabled = false;
        //    //source.FeatureMode = true;
        //    //source.AutomaticGainControl = false;
        //    //source.SystemMode = SystemMode.OptibeamArrayOnly;

        //    RecognizerInfo ri = GetKinectRecognizer();

        //    if (ri == null)
        //    {
        //        Console.WriteLine("Could not find Kinect speech recognizer. Please refer to the sample requirements.");
        //        return;
        //    }

        //    Console.WriteLine("Using: {0}", ri.Name);

        //    sre = new SpeechRecognitionEngine(ri.Id);

        //    var options = new Choices();
        //    options.Add("Start game");
        //    options.Add("Quit");


        //    var gb = new GrammarBuilder();
        //    //Specify the culture to match the recognizer in case we are running in a different culture.                                 
        //    gb.Culture = ri.Culture;
        //    gb.Append(options);


        //    // Create the actual Grammar instance, and then load it into the speech recognizer.
        //    var g = new Grammar(gb);

        //    sre.LoadGrammar(g);
        //    sre.SpeechDetected += SreSpeechDetected;
        //    sre.SpeechRecognized += SreSpeechRecognized;
        //    sre.SpeechHypothesized += SreSpeechHypothesized;
        //    sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

        //    Stream s = source.Start();
        //    sre.SetInputToAudioStream(s,
        //                              new SpeechAudioFormatInfo(
        //                                  EncodingFormat.Pcm, 16000, 16, 1,
        //                                  32000, 2, null));

        //    sre.RecognizeAsync(RecognizeMode.Multiple);
        //}

        //private RecognizerInfo GetKinectRecognizer()
        //{
        //    Func<RecognizerInfo, bool> matchingFunc = r =>
        //    {
        //        string value;
        //        r.AdditionalInfo.TryGetValue("Kinect", out value);
        //        return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
        //    };
        //    return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        //}

        //private void SreSpeechDetected(object sender, SpeechDetectedEventArgs e)
        //{
        //    Console.WriteLine("\nSpeech Detected");

        //}

        //private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        //{
        //    Console.WriteLine("\nSpeech Rejected");

        //}

        //private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        //{
        //    Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        //}

        //private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    //This first release of the Kinect language pack doesn't have a reliable confidence model, so 
        //    //we don't use e.Result.Confidence here.
        //    Console.WriteLine("\nSpeech Recognized: \t{0} {1}", e.Result.Text, e.Result.Confidence);

        //    if (e.Result.Confidence > 0.70f)
        //    {
        //        if (e.Result.Text == "Start game")
        //        {
        //            this.game.gameStatus = Robotech.Game1.GameStatus.Start;
        //        }
        //        else if (e.Result.Text == "Quit")
        //        {
        //            this.Game.Exit();
        //        }
        //    }


        //}

        void LoadGameComponents()
        {
            //Load game
            this.game = this.Game.Services.GetService(typeof(Game1)) as Game1;
            if (this.game == null)
            {
                throw new InvalidOperationException("Game not found.");
            }

            //Load camera
            this.camera = this.Game.Services.GetService(typeof(Camera)) as Camera;
            if (this.camera == null)
            {
                throw new InvalidOperationException("ICameraService not found.");
            }

            ////Load player
            //this.player = this.Game.Services.GetService(typeof(Player)) as Player;
            //if (this.player == null)
            //{
            //    throw new InvalidOperationException("Player not found.");
            //}
        }

        void kinectSensor_AllFrameReady(object sender, AllFramesReadyEventArgs e)
        {
            if (this.kinectSensor == null || !((KinectSensor)sender).SkeletonStream.IsEnabled)
                return;

            bool haveSkeletonData = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (this.skeletonData == null || this.skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                    haveSkeletonData = true;
                }
                if (!haveSkeletonData)
                    return;

                SortedList<float, int> depthSorted = new SortedList<float, int>();
                foreach (Skeleton data in this.skeletonData)
                {
                    if (data.TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        float valueZ = data.Position.Z;
                        while (depthSorted.ContainsKey(valueZ))
                        {
                            valueZ += 0.0001f;
                        }
                        depthSorted.Add(valueZ, data.TrackingId);
                    }
                }
                if (depthSorted.Count == 0)
                    return;

                if (skeletonId == -1 || skeletonId != depthSorted.Values[0])
                {
                    kinectSensor.SkeletonStream.AppChoosesSkeletons = true;
                    skeletonId = depthSorted.Values[0];
                    kinectSensor.SkeletonStream.ChooseSkeletons(skeletonId);
                    //bShowSkeletonTrackVideo = true;
                    //SkeletonTrackVideoTimer = 0;
                }

                foreach (Skeleton data in this.skeletonData)
                {

                    if (skeletonId == data.TrackingId)
                    {

                        if (SkeletonTrackingState.Tracked == data.TrackingState)
                        {
                            SkeletonPoint handRight = data.Joints[JointType.HandRight].Position;
                            SkeletonPoint elbowRight = data.Joints[JointType.ElbowRight].Position;
                            SkeletonPoint shoulderRight = data.Joints[JointType.ShoulderRight].Position;
                            SkeletonPoint hipRight = data.Joints[JointType.HipRight].Position;

                            //cosine law
                            float angleRightY = (float)Math.Acos((Math.Pow(Vector2.Distance(new Vector2(handRight.X, handRight.Z), new Vector2(elbowRight.X, elbowRight.Z)), 2)
                                + Math.Pow(Vector2.Distance(new Vector2(elbowRight.X, elbowRight.Z), new Vector2(shoulderRight.X, shoulderRight.Z)), 2)
                                - Math.Pow(Vector2.Distance(new Vector2(handRight.X, handRight.Z), new Vector2(shoulderRight.X, shoulderRight.Z)), 2))
                                / (2 * Vector2.Distance(new Vector2(handRight.X, handRight.Z), new Vector2(elbowRight.X, elbowRight.Z))
                                * Vector2.Distance(new Vector2(elbowRight.X, elbowRight.Z), new Vector2(shoulderRight.X, shoulderRight.Z))));

                            //cosine law
                            float angleRightX = (float)Math.Acos((Math.Pow(Vector2.Distance(new Vector2(handRight.Y, handRight.Z), new Vector2(shoulderRight.Y, shoulderRight.Z)), 2)
                                + Math.Pow(Vector2.Distance(new Vector2(shoulderRight.Y, shoulderRight.Z), new Vector2(hipRight.Y, hipRight.Z)), 2)
                                - Math.Pow(Vector2.Distance(new Vector2(handRight.Y, handRight.Z), new Vector2(hipRight.Y, hipRight.Z)), 2))
                                / (2 * Vector2.Distance(new Vector2(handRight.Y, handRight.Z), new Vector2(shoulderRight.Y, shoulderRight.Z))
                                * Vector2.Distance(new Vector2(shoulderRight.Y, shoulderRight.Z), new Vector2(hipRight.Y, hipRight.Z))));

                            currRightHandPos.X = handRight.X * 100f;
                            currRightHandPos.Y = handRight.Y * 100f;
                            currRightHandPos.Z = handRight.Z * 100f;

                            currRightHandYAngle = getAngleTrig(handRight.X, handRight.Z);
                            currRightHandXAngle = getAngleTrig(handRight.Y, handRight.Z);

                            //Left hand - zoom
                            SkeletonPoint handLeft = data.Joints[JointType.HandLeft].Position;
                            SkeletonPoint elbowLeft = data.Joints[JointType.ElbowLeft].Position;
                            SkeletonPoint shoulderLeft = data.Joints[JointType.ShoulderLeft].Position;
                            SkeletonPoint hipLeft = data.Joints[JointType.HipLeft].Position;

                            //cosine law
                            float angleLeftY = (float)Math.Acos((Math.Pow(Vector2.Distance(new Vector2(handLeft.X, handLeft.Z), new Vector2(elbowLeft.X, elbowLeft.Z)), 2)
                                + Math.Pow(Vector2.Distance(new Vector2(elbowLeft.X, elbowLeft.Z), new Vector2(shoulderLeft.X, shoulderLeft.Z)), 2)
                                - Math.Pow(Vector2.Distance(new Vector2(handLeft.X, handLeft.Z), new Vector2(shoulderLeft.X, shoulderLeft.Z)), 2))
                                / (2 * Vector2.Distance(new Vector2(handLeft.X, handLeft.Z), new Vector2(elbowLeft.X, elbowLeft.Z))
                                * Vector2.Distance(new Vector2(elbowLeft.X, elbowLeft.Z), new Vector2(shoulderLeft.X, shoulderLeft.Z))));

                            //cosine law
                            float angleLeftX = (float)Math.Acos((Math.Pow(Vector2.Distance(new Vector2(handLeft.Y, handLeft.Z), new Vector2(shoulderLeft.Y, shoulderLeft.Z)), 2)
                                + Math.Pow(Vector2.Distance(new Vector2(shoulderLeft.Y, shoulderLeft.Z), new Vector2(hipLeft.Y, hipLeft.Z)), 2)
                                - Math.Pow(Vector2.Distance(new Vector2(handLeft.Y, handLeft.Z), new Vector2(hipLeft.Y, hipLeft.Z)), 2))
                                / (2 * Vector2.Distance(new Vector2(handLeft.Y, handLeft.Z), new Vector2(shoulderLeft.Y, shoulderLeft.Z))
                                * Vector2.Distance(new Vector2(shoulderLeft.Y, shoulderLeft.Z), new Vector2(hipLeft.Y, hipLeft.Z))));

                            currLeftHandPos.X = handLeft.X * 100f;
                            currLeftHandPos.Y = handLeft.Y * 100f;
                            currLeftHandPos.Z = handLeft.Z * 100f;

                            currLeftHandXAngle = getAngleTrig(handLeft.X, handLeft.Z);
                            currLeftHandYAngle = getAngleTrig(handLeft.Y, handLeft.Z);

                            //Rotate
                            if (MathHelper.ToDegrees(angleLeftY) > 140.0f)
                            {
                                if (MathHelper.ToDegrees(angleLeftX) > 10.0f && MathHelper.ToDegrees(angleLeftX) < 170.0f)
                                {
                                    //YRotation = (prevLeftHandYAngle - currLeftHandYAngle) * Camera.cameraSpeed * 100.0f * 5.0f;
                                    YRotation = (prevLeftHandPos.X - currLeftHandPos.X) * 0.04f;
                                    XRotation = -(prevLeftHandPos.Y - currLeftHandPos.Y) * 0.04f;
                                    this.camera.UpdateCameraLook(0.0f, YRotation, 0.0f);
                                    this.camera.UpdateCameraLook(XRotation, 0.0f, 0.0f);
                                    //Peng after talk we suppress x rotation 
                                    //RotationXModel += (prevHandXAngle - currHandXAngle) * 5;
                                }
                            }

                            //Shoot
                            if (MathHelper.ToDegrees(angleRightX) > 20.0f && MathHelper.ToDegrees(angleRightX) < 170.0f)
                            {
                                if (currRightHandPos.Z >= prevRightHandPos.Z)
                                {
                                    if (!bInShoot)
                                    {
                                        shootStep = 0;
                                        bInShoot = true;
                                    }


                                    if (bInShoot)
                                    {
                                        shootStep += (currRightHandPos.Z - prevRightHandPos.Z);
                                        if (shootStep > shootThreshold)
                                        {
                                            //this.player.ShootEnemy();
                                            bInShoot = false;
                                            shootStep = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    bInShoot = false;
                                    shootStep = 0;
                                }




                            }

                            prevRightHandPos = currRightHandPos;
                            prevRightHandYAngle = currRightHandYAngle;
                            prevRightHandXAngle = currRightHandXAngle;

                            prevLeftHandPos = currLeftHandPos;
                            prevLeftHandXAngle = currLeftHandXAngle;
                            prevLeftHandYAngle = currLeftHandYAngle;
                        }
                        else
                        {
                            skeletonId = -1;
                            //playerId = -1;
                            kinectSensor.SkeletonStream.AppChoosesSkeletons = false;
                        }
                        break;
                    }
                }
            }
        }



        private float getAngleTrig(float x, float y)
        {
            if (x == 0.0f)
            {
                if (y == 0.0f)
                    return (float)((3 * Math.PI) / 2.0f);
                else
                    return (float)(Math.PI / 2.0f);
            }
            else if (y == 0.0f)
            {
                if (x < 0)
                    return (float)Math.PI;
                else
                    return 0;
            }
            if (y > 0.0f)
            {
                if (x > 0.0f)
                    return (float)Math.Atan(y / x);
                else
                    return (float)(Math.PI - Math.Atan(y / -x));
            }
            else
            {
                if (x > 0.0f)
                    return (float)(2.0f * Math.PI - Math.Atan(-y / x));
                else
                {
                    return (float)(Math.PI + (float)Math.Atan(-y / -x));
                }
            }


        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
