using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucene.NET;
using System.Runtime.Serialization;

namespace Lucene.NET.Testing
{
    [TestClass]
    public class StorageTests
    {
        private string entityStorage = @"..\..\Working\Enitites";
        private string index = @"..\..\Working\index";
        [TestInitialize]
        public void Initialize()
        {
            foreach (string file in System.IO.Directory.EnumerateFiles(entityStorage))
            {
                System.IO.File.Delete(file);
            }
            foreach (string file in System.IO.Directory.EnumerateFiles(index))
            {
                System.IO.File.Delete(file);
            }
        }
        

        [TestMethod]
        public void create_storage()
        {
            
            var strategy = new Storage.LuceneStrategy();

            var store = new Storage.LucenceStore<Key, Entity>(entityStorage,strategy,index);
            store.Dispose();
        }
        [TestMethod]
        public void store_an_entity()
        {

            var strategy = new Storage.LuceneStrategy();

            var store = new Storage.LucenceStore<Key, Entity>(entityStorage,strategy,index);
            store.AddOrUpdate(new Key("test", DateTime.Parse("01/01/2012")),
                               () => new Entity(10, 10),
                               (e) => e, 
                               Lokad.Cqrs.AtomicStorage.AddOrUpdateHint.ProbablyExists);


            store.Dispose();
        }
        [TestMethod]
        public void store_an_entity_and_retrieve_it_from_store()
        {
            var strategy = new Storage.LuceneStrategy();

            var store = new Storage.LucenceStore<Key, Entity>(entityStorage, strategy, index);
            var key = new Key("test",DateTime.Parse("01/01/2012"));

            store.AddOrUpdate(key,
                               () => new Entity(10, 10),
                               (e) => e,
                               Lokad.Cqrs.AtomicStorage.AddOrUpdateHint.ProbablyExists);
            Entity entity;

            Assert.IsTrue(store.TryGet(key,out entity));

            Assert.IsTrue(entity.ItemCount == 10);
            Assert.IsTrue(entity.TotalAmount == 10);



            store.Dispose();
        }
    }
    

    public class Key
    {
        private string _tenantId;
        private DateTime _created;

        public Key(string tenantId, DateTime created)
        {
            _tenantId = tenantId;
            _created = created;
        }

        public string TenantId {get {return _tenantId; }}
        public string Created { get { return _created.ToShortDateString(); } }
    }
    [DataContract(Namespace = "DDDSample")]
    public class Entity
    {
        public Entity() { }
        public Entity(int itemCount, decimal totalAmount)
        {
            this.ItemCount = itemCount;
            this.TotalAmount = totalAmount;
        }
        [DataMember(Order = 1)]
        public int ItemCount { get; private set; }
        [DataMember(Order = 2)]
        public decimal TotalAmount { get; private set; }


    }
}
