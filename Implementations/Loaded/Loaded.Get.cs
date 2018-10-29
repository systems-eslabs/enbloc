using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Enbloc.DbEntities;


namespace Enbloc
{
    public partial class Loaded
    {

        public LoadedEnbloc getEnbloc(string enblocNumber)
        {
            return new EmpezarRepository<LoadedEnbloc>().Find(enbloc => enbloc.EnblocNumber == enblocNumber);
        }

        public List<LoadedEnbloc> getAllEnbloc()
        {
            return new EmpezarRepository<LoadedEnbloc>().GetList(enbloc => enbloc.Status != Status.COMPLETED).ToList();
        }

        public List<LoadedEnbloc> getDashboardEnbloc()
        {
            return new EmpezarRepository<LoadedEnbloc>().GetList(enbloc => (enbloc.CompletedDate < DateTime.Now.AddDays(1))).ToList();
        }

        //Form Top Start the Enbloc
        public List<LoadedEnbloc> getAllPendingEnbloc()
        {
            return new EmpezarRepository<LoadedEnbloc>().GetList(enbloc => enbloc.Status == Status.PENDING).ToList();
        }

        public List<LoadedEnblocContainers> getEnblocContainer(string enblocNumber)
        {
            return new EmpezarRepository<LoadedEnblocContainers>().GetList(enblocContainer => enblocContainer.EnblocNumber == enblocNumber).ToList();
        }


        public void startEnbloc(string enblocNumber,DateTime startDate)
        {

            var _repository = new EmpezarRepository<LoadedEnbloc>();
            var enblocEntity = _repository.Find(enbloc => enbloc.EnblocNumber == enblocNumber);

            if (enblocEntity != null)
            {
                enblocEntity.ModifiedDate = DateTime.Now;
                enblocEntity.ModifiedBy = 0; //Get UserId from JWT 
                
                if(startDate.ToString("dd-MMM-yy") == DateTime.Now.ToString("dd-MMM-yy")){
                    enblocEntity.Status = Status.INPROGRESS;                   
                }               
                enblocEntity.StartDate = startDate;
                _repository.Update(enblocEntity);
            }

        }

        // 

        //Stop Enbloc

        //Push - Update Hold

        //Push - Update Approved 

       
    }
}