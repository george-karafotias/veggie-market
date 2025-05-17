using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.DbServices;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class DataStorageService
    {
        private static DataStorageService dataStorageServiceInstance = null;
        private ILogger logger;
        private readonly DbService dbService;
        private readonly ProductTypeDbService productTypeDbService;
        private readonly MarketDbService marketDbService;
        private readonly ProductDbService productDbService;
        private readonly ProductPriceDbService productPriceDbService;
        private readonly ProcessedProductPriceDbService processedProductPriceDbService;
        private readonly MetadataDbService metadataDbService;

        private DataStorageService(DbService dbService, ILogger logger)
        {
            this.logger = logger;
            this.dbService = dbService;
            productTypeDbService = new ProductTypeDbService(this.dbService);
            marketDbService = new MarketDbService(this.dbService);
            productDbService = new ProductDbService(this.dbService);
            productPriceDbService = new ProductPriceDbService(this.dbService);
            processedProductPriceDbService = new ProcessedProductPriceDbService(this.dbService);
            metadataDbService = new MetadataDbService(this.dbService, marketDbService);
        }

        public static DataStorageService GetInstance(DbService dbService, ILogger logger)
        {
            if (dataStorageServiceInstance == null) 
            {
                dataStorageServiceInstance = new DataStorageService(dbService, logger);
                dataStorageServiceInstance.CreateDatabase();
            }
            return dataStorageServiceInstance;
        }

        public ProductTypeDbService ProductTypeDbService { get { return productTypeDbService; } }

        public MarketDbService MarketDbService { get { return marketDbService; } }

        public ProductDbService ProductDbService { get { return productDbService; } }

        public ProductPriceDbService ProductPriceDbService {  get { return productPriceDbService; } }

        public ProcessedProductPriceDbService ProcessedProductPriceDbService { get{ return processedProductPriceDbService; } }

        public MetadataDbService MetadataDbService { get { return metadataDbService; } }

        public ILogger Logger { get { return logger; } set { this.logger = value; } }

        public DbService DbService { get { return dbService; } }

        private void CreateDatabase()
        {
            dbService.CreateDatabase();
        }
    }
}
