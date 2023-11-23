namespace ConsiditionLib2023
{
    public class SolutionConfiguration
    {
        public SolutionConfiguration(Maps map, Dictionary<string, PlacedLocations> locations)
        {
            Map = map;
            Locations = locations;
        }

        public Maps Map
        {
            get;
            private set;
        }

        /// <summary>
        /// Denna ska optimeras med antalet automater i <see cref="PlacedLocation"/>
        /// </summary>
        public Dictionary<string, PlacedLocations> Locations
        {
            get;
            private set;
        }
    }
}
