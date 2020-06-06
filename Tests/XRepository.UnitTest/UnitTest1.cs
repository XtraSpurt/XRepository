using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XRepository.EntityFramework.Shared;
using Xunit;

namespace XRepository.UnitTest
{
    public class UnitTest1 : IClassFixture<DbFixture>
    {

        private ServiceProvider _serviceProvide;

        public UnitTest1(DbFixture fixture)
        {
            _serviceProvide = fixture.ServiceProvider;
        }


        [Fact]
        public async Task Test1()
        {
            await using var context = _serviceProvide.GetService<XDbContext>();

           
                await context.Models.AddAsync(new XModel()
                {
                    Name = "XModel",
                    Number = 55555
                });
                await context.SaveChangesAsync();

                var list = await context.Models.ToListAsync();

                var listCount = list.Count; 
                Assert.NotNull(list);
                Assert.Equal(1,listCount);
            
        }
    }




    public class DbFixture
    {
        public DbFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterInMemoryXDbContexts(); 
               
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }

}
