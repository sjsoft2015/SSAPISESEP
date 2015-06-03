using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.ModAPI;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Logger;

namespace SSAPISESEP
{
    [Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.BeforeSimulation)]
    public class Core : Sandbox.Common.MySessionComponentBase
    {
        private int delayStart = 200; // updates before starting
        private const int FRAMES_BETWEEN_UPDATES = 10;
        private int updateCount = 0;
        private static Log log = null;

        protected override void UnloadData()
        {
            if (log != null)
            {
                log.WriteLine("UnloadData()");
                log.Close();
                log = null;
            }
        }
 
        //
        // == SessionComponent Hooks
        //
        public override void UpdateBeforeSimulation()
        {
            if (delayStart > 0)
            {
                delayStart--;
                return;
            }
            if (log == null)
                log = new Log("debug.log");
            log.WriteLine(string.Format("{0} - UpdateBeforeSimulation", updateCount));
            try
            {

                if (updateCount % FRAMES_BETWEEN_UPDATES == 0)
                {
                    log.WriteLine("SSAPISESEP MAIN doUpdate...");

                    List<IMyPlayer> players = new List<IMyPlayer>();
                    MyAPIGateway.Players.GetPlayers(players);
                    foreach (IMyPlayer obj in players)
                    {
                        log.WriteLine(string.Format("player := {0}", obj.DisplayName.ToString()));
                        if (obj.Controller.ControlledEntity.Entity is MyCharacter)
                        {
                            MyObjectBuilder_Character mc = (MyObjectBuilder_Character)obj.Controller.ControlledEntity.Entity.GetObjectBuilder(false);
                            log.WriteLine(string.Format("   JetpackEnabled {0}", mc.JetpackEnabled));
                            if (mc.JetpackEnabled)
                            {
                                ((IMyPlayer)obj).Controller.ControlledEntity.SwitchThrusts();
                            }
                        }

                    }
                }
            }
            catch (Exception coreEx)
            {
                log.WriteLine("Exception in core: " + coreEx);
            }
            updateCount++;
        }

    }

}
