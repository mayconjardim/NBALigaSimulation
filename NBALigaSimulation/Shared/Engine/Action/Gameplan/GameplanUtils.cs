namespace NBALigaSimulation.Shared.Engine.Action.Gameplan
{
    public static class GameplanUtils
    {

        public static double GameplanPace(int value)
        {
            switch (value)
            {
                case 1:
                    return 95.7;
                case 2:
                    return 98.0;
                default:
                    return 101.6;
            }
        }



    }
}
