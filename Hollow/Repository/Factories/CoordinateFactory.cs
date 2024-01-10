using NetTopologySuite.Geometries;

namespace Repository
{
    public class CoordinateFactory
    {
        private readonly GeometryFactory internalFactory;

        public CoordinateFactory()
        {
            //4326
            NetTopologySuite.NtsGeometryServices.Instance = new NetTopologySuite.NtsGeometryServices
                (
                    NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                    new PrecisionModel(1000d),
                    4326,   
                    GeometryOverlay.NG,
                    new CoordinateEqualityComparer()
                );

            internalFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        }

        public Point Create(double longitude, double latitude)
        {         
            return internalFactory.CreatePoint(new Coordinate(longitude, latitude));
        }
    }
}
