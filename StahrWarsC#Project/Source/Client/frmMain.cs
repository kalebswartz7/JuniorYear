using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
	public partial class frmMain : Form {
        int sector = 0, col = 3, row = 7, shipAngle = 0, boxCount = 10, health = 100, fuel = 50, phasorCount = 50, torpedoCount = 10;
        bool phasors = true;
        List<int> rows = new List<int>();
        List<int> cols = new List<int>();
        List<int> angles = new List<int>();
        string playerData = "";
        static string region = "";
		bool gameOn = false, shieldOn = false, drawObjects = false;
        string data = "";
		UdpUser client = null;
		Pen gridPen = new Pen(System.Drawing.Color.White, 1);
		int gridSize = 0;
		Image planet = Image.FromFile("jupiter.png");
		Image star = Image.FromFile("star.png");
        Image blackhole = Image.FromFile("theoreticala.jpg");
		Image background = Image.FromFile("background.jpg");

        Image shipNorth = Image.FromFile("ShipNorth.png");
        Image shipSouth = Image.FromFile("ShipSouth.png");
        Image shipEast = Image.FromFile("ShipEast.png");
        Image shipWest = Image.FromFile("ShipWest.png");


        #region ====================================================================================== UDP Class Setup

        // Structure
        public struct Received {
			public IPEndPoint Sender;
			public string Message;
		}

		// UDP Abstract class
		abstract class UdpBase {
			protected UdpClient Client;

			protected UdpBase() {
				Client = new UdpClient();
			}

			public async Task<Received> Receive() {
				var result = await Client.ReceiveAsync();
				return new Received() {
					Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
					Sender = result.RemoteEndPoint
				};
			}
		}

		// Client
		class UdpUser : UdpBase {
			private UdpUser() { }

			public static UdpUser ConnectTo(string hostname, int port) {
				var connection = new UdpUser();
				connection.Client.Connect(hostname, port);
				return connection;
			}

			public void Send(string message) {
				var datagram = Encoding.ASCII.GetBytes(message);
				Client.Send(datagram, datagram.Length);
			}
		}

        #endregion

        public frmMain() {
			InitializeComponent();

            // set up the grid look - 1 dot, 4 spaces, 1 dot, 4 spaces
            gridPen.DashPattern = new float[] { 1, 4 };
			gridSize = panCanvas.Width / boxCount;


		}

		private void beginMessages() {
			if (txtIP.Text.Trim().Length == 0) {
				MessageBox.Show("You must enter the IP address of the server",
									"Connection Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			client = UdpUser.ConnectTo(txtIP.Text.Trim(), 32123);
			client.Send("Connected: " + Environment.UserName);
            //addText("Connected: " + Environment.UserName);

			// 10.33.7.192

			string msg;
			string[] parts;
			Task.Factory.StartNew(async () => {
				while (true) {
					try {
						msg = (await client.Receive()).Message.ToString();

						parts = msg.Split(':');
						fixParts(parts);
						if (parts[0].Equals("quit")) {
							break;
						} else if (parts[0].Equals("connected")) {
							gameOn = parts[1].Equals("true");

							if (gameOn) {
								txtIP.Invoke(new Action(() => txtIP.BackColor = Color.Green));
								btnConnect.Invoke(new Action(() => btnConnect.BackColor = Color.Green));
								txtIP.Invoke(new Action(() => txtIP.ReadOnly = true));
								txtIP.Invoke(new Action(() => txtIP.Enabled = false));
								btnConnect.Invoke(new Action(() => btnConnect.Enabled = false));

								addText("You have joined the game...\n");
                                region = Convert.ToString(parts[2]);
								sector = Convert.ToInt32(parts[3]);
								col = Convert.ToInt32(parts[4]);
                                row = Convert.ToInt32(parts[5]);
                                lblSector.Invoke(new Action(() => lblSector.Text = region.ToUpper() + "-" + sector));
                                prbHealth.Invoke(new Action(() => prbHealth.Value = 100));
                                FuelBar.Invoke(new Action(() => FuelBar.Value = 50));
                                torpedoBar.Invoke(new Action(() => torpedoBar.Value = 10));
                                phasorBar.Invoke(new Action(() => phasorBar.Value = 50));
                            } else {
								addText("Connection not established\n");
								sector = row = col = -1;
							}

							panCanvas.Invoke(new Action(() => panCanvas.Refresh()));

						} else if(parts[0].Equals("loc")) {
                            string playerData = parts[1];
                            string[] realData = playerData.Split('-');
                            region = Convert.ToString(realData[0]);
                            sector = Convert.ToInt32(realData[1]);
                            row = Convert.ToInt32(realData[2]);
                            col = Convert.ToInt32(realData[3]);
                            health = Convert.ToInt32(realData[4]);
                            fuel = Convert.ToInt32(realData[5]);
                            phasorCount = Convert.ToInt32(realData[6]);
                            torpedoCount = Convert.ToInt32(realData[7]);

                            FuelBar.Invoke(new Action(() => FuelBar.Value = fuel));
                            if (health <= 0)
                            {
                                MessageBox.Show(new Form() { TopMost = true }, "You have died.");

                            }
                            prbHealth.Invoke(new Action(() => prbHealth.Value = health));
                            prbHealth.Invoke(new Action(() => phasorBar.Value = phasorCount));
                            prbHealth.Invoke(new Action(() => torpedoBar.Value = torpedoCount));
                            lblSector.Invoke(new Action(() => lblSector.Text = region.ToUpper() + "-" + sector));
                            panCanvas.Invoke(new Action(() => panCanvas.Refresh()));

                        }
                        else if (parts[0].Equals("loc2"))
                        {
                            rows = new List<int>();
                            cols = new List<int>();
                            angles = new List<int>();
                            string playerData = parts[1];
                            string[] realData = playerData.Split('-');
                            region = Convert.ToString(realData[0]);
                            sector = Convert.ToInt32(realData[1]);
                            row = Convert.ToInt32(realData[2]);
                            col = Convert.ToInt32(realData[3]);
                            health = Convert.ToInt32(realData[4]);
                            fuel = Convert.ToInt32(realData[5]);
                            phasorCount = Convert.ToInt32(realData[6]);
                            torpedoCount = Convert.ToInt32(realData[7]);
                            string otherData = parts[2];
                            string[] realData2 = otherData.Split('-');
                                for (int i = 0; i < realData2.Length - 1; i++)
                                {
                                    int k = Convert.ToInt32(realData2[i]);
                                    rows.Add(k);
                                    int k2 = Convert.ToInt32(realData2[i + 1]);
                                    cols.Add(k2);
                                    int k3 = Convert.ToInt32(realData2[i + 2]);
                                    angles.Add(k3);
                                    i = i + 2;
                                }
                            


                            FuelBar.Invoke(new Action(() => FuelBar.Value = fuel));
                            if (health <= 0)
                            {
                                addText("A");
                                MessageBox.Show(new Form() { TopMost = true }, "You have died.");

                            }
                            prbHealth.Invoke(new Action(() => prbHealth.Value = health));
                            prbHealth.Invoke(new Action(() => phasorBar.Value = phasorCount));
                            lblSector.Invoke(new Action(() => lblSector.Text = region.ToUpper() + "-" + sector));
                            prbHealth.Invoke(new Action(() => torpedoBar.Value = torpedoCount));
                            panCanvas.Invoke(new Action(() => panCanvas.Refresh()));
                        }
                        else if (parts[0].Equals("phasors"))
                        {
                            FuelBar.Invoke(new Action(() => phasorBar.Value = Convert.ToInt32(parts[1])));
                        }
                        else if (parts[0].Equals("universe"))
                        {
                            addText("reached");
                            addText(parts[1].ToUpper());
                        }
                        else if (parts[0].Equals("display"))
                        {
                            addText(parts[1]);
                        }
                        else if (parts[0].Equals("torpedos"))
                        {
                            FuelBar.Invoke(new Action(() => torpedoBar.Value = Convert.ToInt32(parts[1])));
                        }
                        else if (parts[0].Equals("healthMessage"))
                        {
                            MessageBox.Show(new Form() { TopMost = true }, "You have died.");
                        }

                        else {
                            data = msg;

						}
					} catch (Exception e) {
						MessageBox.Show(e.Message);
					}
				}

				Environment.Exit(0);
			});
		}

		private void panCanvas_Paint(object sender, PaintEventArgs e) {
			if (gameOn) {
			    if (chkShowBackground.Checked)
				    e.Graphics.DrawImage(background, 0, 0);
			    if (chkShowGrid.Checked) {
				    // Draw the grid
				    for (int i = gridSize; i < panCanvas.Height; i += gridSize) {
					    e.Graphics.DrawLine(gridPen, 0, i, panCanvas.Width, i);
					    e.Graphics.DrawLine(gridPen, i, 0, i, panCanvas.Height);
				    }
			    }
                    
               for (int i = 0; i < data.Length; i++)
                {
                    var chars = data.ToCharArray();
                    string r1 = chars[i].ToString();
                    string r2 = chars[i+1].ToString();
                    string r3 = chars[i + 3].ToString();
                    int r = Convert.ToInt32(r1);
                    int c = Convert.ToInt32(r2);
                    if (r3.Equals("1"))
                    {
                        e.Graphics.DrawImage(star, loc(c, r, star.Width / 4));

                    }
                    else if (r3.Equals("2"))
                    {
                        e.Graphics.DrawImage(planet, loc(c, r, gridSize / 1.5));

                    }
                    else if (r3.Equals("3"))
                    {
                        e.Graphics.DrawImage(blackhole, loc(c, r, gridSize / 1.5));

                    }
                    i = i + 3;
                }


                if (shipAngle == 0) e.Graphics.DrawImage(shipNorth, loc(col, row, shipNorth.Width / 2));
                else if (shipAngle == 90) e.Graphics.DrawImage(shipEast, loc(col, row, shipEast.Width / 2));
                else if (shipAngle == 180) e.Graphics.DrawImage(shipSouth, loc(col, row, shipSouth.Width / 2));
                else e.Graphics.DrawImage(shipWest, loc(col, row, shipWest.Width / 2));

                if (shieldOn)
                    e.Graphics.DrawEllipse(new Pen(Brushes.Gold, 2), loc(col, row, gridSize / 1.5));

                if (rows.Count > 0)
                {

                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (angles[i] == 0) e.Graphics.DrawImage(shipNorth, loc(cols[i], rows[i], shipNorth.Width / 2));
                        else if (angles[i] == 90) e.Graphics.DrawImage(shipEast, loc(cols[i], rows[i], shipNorth.Width / 2));
                        else if (angles[i] == 180) e.Graphics.DrawImage(shipSouth, loc(cols[i], rows[i], shipNorth.Width / 2));
                        else if (angles[i] == 270) e.Graphics.DrawImage(shipWest, loc(cols[i], rows[i], shipNorth.Width / 2));
                    }
                }


            }
        }

      
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (!gameOn) return false;
			try {
				switch (keyData) {
					case Keys.Up:
						if (shipAngle != 0) {
							shipAngle = 0;
							panCanvas.Refresh();
							client.Send("update0");
						} else {
                            move("n");
                            rows.Clear();
                            cols.Clear();
                            angles.Clear();
                        }
						break;
					case Keys.Right:
						if (shipAngle != 90) {
							shipAngle = 90;
							panCanvas.Refresh();
							client.Send("update90");
						} else {
                            move("e");
                            rows.Clear();
                            cols.Clear();
                            angles.Clear();
                        }
						break;
					case Keys.Down:
						if (shipAngle != 180) {
							shipAngle = 180;
							panCanvas.Refresh();
							client.Send("update180");
						} else {
                            move("s");
                            rows.Clear();
                            cols.Clear();
                            angles.Clear();
                        }
						break;
					case Keys.Left:
						if (shipAngle != 270) {
							shipAngle = 270;
							panCanvas.Refresh();
							client.Send("update270");
						} else {
                            move("w");
                            rows.Clear();
                            cols.Clear();
                            angles.Clear();
                        }
						break;
					case Keys.S:
                        switchShields();
                        client.Send("shieldOn");
                        break;
                    case Keys.H:
                        client.Send("mov:h");
                        rows.Clear();
                        cols.Clear();
                        angles.Clear();
                        break;
                    case Keys.F:
                        if (phasors)
                        {
                            client.Send("fireP");
                        }
                        else
                        {
                            client.Send("fireT");
                        }
                        break;
                    case Keys.U:
                        client.Send("viewu");
                        break;
                }
			} catch (Exception ex) {
				this.Text = ex.Message;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

        private void move(string dir) {
            if (!shieldOn) client.Send("mov:" + dir);
        }

        #region ====================================================================================== <Shields>

        private void switchShields() {
            if (!gameOn) return;
            shieldOn = !shieldOn;
            client.Send("s" + (shieldOn ? "1" : "0"));
            lblShielsUp.ForeColor = (shieldOn ? Color.Green : Color.Red);
            lblShielsUp.Text = (shieldOn ? "ON" : "OFF");
            panCanvas.Refresh();
            picShields.Refresh();
        }
        private void switchWeapon()
        {
            phasors = !phasors;
        }

        private void picShields_Click(object sender, EventArgs e) {
            switchShields();
            client.Send("shieldOn");
        }

        private void lblShielsUp_Click(object sender, EventArgs e) {
            switchShields();
            client.Send("shieldOn");
        }

        private void picShields_Paint(object sender, PaintEventArgs e) {
            e.Graphics.FillEllipse(shieldOn ? Brushes.Green : Brushes.Red, 2, 2, picShields.Width - 4, picShields.Height - 4);
            e.Graphics.DrawEllipse(Pens.Gray, 2, 2, picShields.Width - 4, picShields.Height - 4);
        }

        #endregion


        #region ====================================================================================== <Local Settings>

        private void chkShowGrid_CheckedChanged(object sender, EventArgs e) {
			panCanvas.Refresh();
		}

        private void chkShowBackground_CheckedChanged(object sender, EventArgs e) {
            panCanvas.Refresh();
        }

        private void prbHealth_Click(object sender, EventArgs e)
        {

        }

        private void grpStatus_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            switchWeapon();
            
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region ====================================================================================== <Local Methods>

        private void addText(string msg) {
			txtServerMessages.Invoke(new Action(() => txtServerMessages.ReadOnly = false));
			txtServerMessages.Invoke(new Action(() => txtServerMessages.AppendText(" > " + msg + "\n")));
			txtServerMessages.Invoke(new Action(() => txtServerMessages.ReadOnly = true));
			this.Invoke(new Action(() => this.Focus()));
		}

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private Rectangle loc(double col, double row, double size) {
			double offset = (gridSize - size) / 2;
			return new Rectangle((int)(col * gridSize + offset),
									(int)(row * gridSize + offset),
									(int)size,
									(int)size);
		}

        private static void fixParts(string[] parts) {
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim().ToLower();
        }

        #endregion


        #region ====================================================================================== Local Button Clicks

        private void btnConnect_Click(object sender, EventArgs e) {
            beginMessages();
        }

        private void btnQuit_Click(object sender, EventArgs e) {
            this.Close();
        }

        #endregion


        #region ====================================================================================== Closing Application

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
			if (client != null) client.Send("quit");
		}

        #endregion

    }
}

