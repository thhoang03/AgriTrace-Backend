using AgriTrace.Infrastructure.Sqlserver.Models;


namespace AgriTrace.Infrastructure.Sqlserver.Persistence;


public static class SeedData
{

    public static void Seed(
        ApplicationDbContext context)
    {


        context.Database.EnsureCreated();



        SeedOrganizationTypes(context);


        SeedEventTypes(context);



        context.SaveChanges();

    }



    private static void SeedOrganizationTypes(
        ApplicationDbContext context)
    {

        if (context.OrganizationTypes.Any())
            return;



        context.OrganizationTypes.AddRange(

            new OrganizationTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "FARM",
                Name = "Farm",
                Description = "Agricultural Farm"
            },


            new OrganizationTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "PROCESSOR",
                Name = "Processor"
            },


            new OrganizationTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "DISTRIBUTOR",
                Name = "Distributor"
            },


            new OrganizationTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "RETAILER",
                Name = "Retailer"
            }

        );

    }





    private static void SeedEventTypes(
        ApplicationDbContext context)
    {

        if (context.EventTypes.Any())
            return;



        context.EventTypes.AddRange(

            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "HARVEST",
                Name = "Harvest"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "PROCESSING",
                Name = "Processing"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "PACKAGING",
                Name = "Packaging"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "TRANSPORT",
                Name = "Transport"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "DISTRIBUTION",
                Name = "Distribution"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "RETAIL",
                Name = "Retail"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "INSPECTION",
                Name = "Inspection"
            },


            new EventTypeDataModel
            {
                Id = Guid.NewGuid(),
                Code = "RECALL",
                Name = "Recall"
            }

        );

    }

}