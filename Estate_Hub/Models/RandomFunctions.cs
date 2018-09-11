using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proptiwise.Models
{
    public class RandomFunctions
    {
        public static List<Property> GetPropertiesList(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.Properties.Where(p => p.Landlord_Id == lid).ToList();
                return cnt;
            };

        }
        public static List<Property> GetVacantPropertiesList(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.Properties.Where(p => p.Landlord_Id == lid && p.leasestatus == 0).ToList();
                return cnt;
            };

        }
        public static List<Property> GetLeasedPropertiesList(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.Properties.Where(p => p.Landlord_Id == lid && p.leasestatus == 1).ToList();
                return cnt;
            };

        }

        public static List<Tenant> GetTenantList(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.Tenants.Where(p => p.Property_Id == lid).ToList();
                return cnt;
            };
        }

        public static int GetPropertiesOfBuilding(int? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                int cnt = dv.Properties.Where(p => p.BuildingId == bid).Count();
                return cnt;
            };

        }
        public static List<Property> GetPropertiesData(int? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.Properties.Where(p => p.BuildingId == bid).ToList();
                return cnt;
            };

        }

        public static tbl_lease getleasedata(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.tbl_lease.Where(p => p.Property_Id == bid).FirstOrDefault();
                return cnt;
            };

        }
        public static Property getrequestpropdata(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Properties.Where(p => p.Property_Id == bid).FirstOrDefault();
                return cnt1;
            };
        }

        public static List<RequestView> GetApprovedRequestd(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.RequestViews.Where(p => p.TenantId == bid && p.Approve != 0 && p.Approve != 4 && p.Approve != 3).ToList();
                return cnt1;
            };

        }

        public static List<RequestView> GetApprovedRequestdlandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.RequestViews.Where(p => p.LandlordId == bid && p.Approve == 0).ToList();
                return cnt1;
            };

        }
        public static List<tbl_invoice> getinvoicedata(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.tbl_invoice.Where(p => p.LandlordId == bid).ToList();
                return cnt1;
            };

        }
        public static List<MovingIn> moveindata(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.MovingIns.Where(p => p.LandlordId == bid).ToList();
                return cnt1;
            };

        }
        public static List<tbl_invoice> getinvoicedatatenant(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.tbl_invoice.Where(p => p.TenantId == bid).ToList();
                return cnt1;
            };

        }
        public static List<MovingIn> moveindatatenant(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.MovingIns.Where(p => p.TenantId == bid).ToList();
                return cnt1;
            };

        }
        public static List<tbl_offer> GetApprovedOffers(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.tbl_offer.Where(p => p.TenantId == bid && p.Approval != 0 && p.Approval != 4 && p.Approval != 3).ToList();
                return cnt1;
            };

        }
        public static List<tbl_offer> GetApprovedOfferslandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.tbl_offer.Where(p => p.LandlordId == bid && p.Approval == 0).ToList();
                return cnt1;
            };

        }
        public static List<ViewingPurposal> GetPendingViewingProposals(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.ViewingPurposals.Where(p => p.TenantId == bid && p.Approve == 0).ToList();
                return cnt1;
            };

        }
        public static List<ViewingPurposal> GetPendingViewingProposalslandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.ViewingPurposals.Where(p => p.LandlordId == bid && p.Approve != 0 && p.Approve != 4 && p.Approve != 3).ToList();
                return cnt1;
            };

        }
        public static List<application_details> getpendingapplicationreq(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.application_details.Where(p => p.TenantId == bid && p.Status == 0).ToList();
                return cnt1;
            };

        }
        public static List<application_details> getpendingapplicationreqlandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.application_details.Where(p => p.LandlordId == bid && p.Status != 0 && p.Status != 4 && p.Status != 3).ToList();
                return cnt1;
            };

        }
        public static List<TenancyAgreement> gettenantagreement(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.TenancyAgreements.Where(p => p.TenantId == bid && p.ApprovalStatus == 0).ToList();
                return cnt1;
            };

        }
        public static List<TenancyAgreement> gettenantagreementlandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.TenancyAgreements.Where(p => p.LandlordId == bid && p.ApprovalStatus != 0 && p.ApprovalStatus != 4 && p.ApprovalStatus != 3).ToList();
                return cnt1;
            };

        }

        public static List<Purposal> GetPendingProposals(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Purposals.Where(p => p.TenantId == bid && p.Approval == 0).ToList();
                return cnt1;
            };

        }
        public static List<Purposal> GetPendingProposalslandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Purposals.Where(p => p.LandlordId == bid && p.Approval != 0 && p.Approval != 4 && p.Approval != 3).ToList();
                return cnt1;
            };

        }
        public static List<Deposit> GetPendingDepositReq(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Deposits.Where(p => p.TenantId == bid && p.DepositApproval == 0).ToList();
                return cnt1;
            };

        }
        public static List<Deposit> GetPendingDepositReqlandlord(long? bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Deposits.Where(p => p.LandlordId == bid && p.DepositApproval != 0 && p.DepositApproval != 4 && p.DepositApproval != 3).ToList();
                return cnt1;
            };

        }
        public static PropertyImage GetFirstImage(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.PropertyImages.Where(p => p.PropertyId == pid).FirstOrDefault();
                return cnt1;
            };

        }

        public static RequestView GetRequestViewing(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.RequestViews.Where(p => p.PropertyId == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static User getlandlordname(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Users.Where(p => p.UserID == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static Property getpropertyname(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Properties.Where(p => p.Property_Id == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static Tenant gettenantname(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Tenants.Where(p => p.Tenant_Id == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static FaultType getfaultname(int? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.FaultTypes.Where(p => p.FaultTypeId == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static tbl_offer GetOffer(long? pid)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.tbl_offer.Where(p => p.PropertyId == pid).FirstOrDefault();
                return cnt1;
            };

        }
        public static string getinventname(string bid)
        {
            using (var dv = new dbrealestateEntities())
            {
                int id = Convert.ToInt32(bid);
                string cnt = dv.tbl_inventory.Where(p => p.InventoryId == id).Select(p => p.Name).FirstOrDefault();
                return cnt;
            };

        }
        public static List<Fault> getExpenditure(long? prpid)
        {
            using (var dv = new dbrealestateEntities())
            {
                //  dv.Configuration.LazyLoadingEnabled = true;
                List<Fault> cnt = dv.Faults.Where(p => p.PropertyId == prpid).ToList();
                return cnt;
            };
        }
        public static FaultType getFaultType(int? tid)
        {
            using (var dv = new dbrealestateEntities())
            {
                //  dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.FaultTypes.Where(p => p.FaultTypeId == tid).FirstOrDefault();
                return cnt;
            };
        }

        public static List<TabRole> GetTabs()
        {
            using (var dv = new dbrealestateEntities())
            {
                //  dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.TabRoles.ToList();
                return cnt;
            };
        }
        public static List<ReportRole> GetReportNames()
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.ReportRoles.ToList();
                return cnt;
            };
        }
        public static List<UserTabPermission> GetTabPermissions(long? id)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.UserTabPermissions.Where(p => p.UserId == id).ToList();
                return cnt;
            };
        }

        public static List<UserReportsPermission> GetReportPermissions(long? id)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt = dv.UserReportsPermissions.Where(p => p.UserId == id).ToList();
                return cnt;
            };
        }

        public static List<paymentHistory> GetPaymentDue(long? lid)
        {
            DateTime dtt = DateTime.Now.AddMonths(-1);
            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.paymentHistories.Where(p => p.DueDate >= dtt && p.DueDate <= DateTime.Now && p.Landlord_id == lid).ToList();
                return cnt;
            };
        }
        public static List<tbl_lease> GetRecentLease(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                //  dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.tbl_lease.Where(p => p.Property.Landlord_Id == lid).OrderByDescending(p => p.LeaseId).Take(3).ToList();
                return cnt;
            };
        }
        public static List<Fault> GetWorkOrders(long? lid)
        {
            using (var dv = new dbrealestateEntities())
            {
                //  dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.Faults.OrderByDescending(p => p.FaultId).Where(p => p.LandlordId == lid).Take(3).ToList();
                return cnt;
            };
        }
        public static List<tbl_lease> GetLateRent(int? drpval, long? lid)
        {
            DateTime addsevendays = DateTime.Now.AddDays(7), addfourteendays = DateTime.Now.AddDays(14), addthirtydays = DateTime.Now.AddDays(30);
            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.tbl_lease.Where(p => p.DueDate < DateTime.Now && p.Property.Landlord_Id == lid).ToList();
                if (drpval == 10)
                    cnt = cnt.Where(p => p.DueDate <= addthirtydays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 9)
                    cnt = cnt.Where(p => p.DueDate <= addfourteendays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 8)
                    cnt = cnt.Where(p => p.DueDate <= addsevendays && p.Property.Landlord_Id == lid).ToList();
                return cnt;
            };
        }
        public static List<tbl_lease> GetExpiringLease(int? drpval, long? lid)
        {
            DateTime addsevendays = DateTime.Now.AddDays(7), addfourteendays = DateTime.Now.AddDays(14), addthirtydays = DateTime.Now.AddDays(30), addsixtydays = DateTime.Now.AddDays(60), addonetwntydays = DateTime.Now.AddDays(120);
            using (var dv = new dbrealestateEntities())
            {
                //    dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.tbl_lease.Where(p => p.Property.Landlord_Id == lid).ToList();
                if (drpval == 0)
                    cnt = cnt.Where(p => p.EndDate <= addthirtydays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 1)
                    cnt = cnt.Where(p => p.EndDate <= addfourteendays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 2)
                    cnt = cnt.Where(p => p.EndDate <= addsevendays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 3)
                    cnt = cnt.Where(p => p.EndDate <= addsixtydays && p.Property.Landlord_Id == lid).ToList();
                else if (drpval == 120)
                    cnt = cnt.Where(p => p.EndDate <= addonetwntydays && p.Property.Landlord_Id == lid).ToList();
                return cnt;
            };
        }
        public static List<tbl_lease> NextPaymentDue(long? tid)
        {
            DateTime addsevendays = DateTime.Now.AddDays(7);
            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var cnt = dv.tbl_lease.Where(p => p.DueDate <= addsevendays && p.primaryTenant_Id == tid).ToList();
                return cnt;
            };
        }
        public static List<setting> getcountries()
        {
            DateTime addsevendays = DateTime.Now.AddDays(7);
            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var countries = dv.settings.ToList();
                return countries;
            };
        }
        public static List<tbMessage> getMessages(long? lid)
        {
            DateTime addsevendays = DateTime.Now.AddDays(7);

            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var landlord = dv.Users.Where(p => p.UserID == lid).FirstOrDefault();
                var messages = dv.tbMessages.Where(p => p.emailto == landlord.Email).OrderByDescending(p => p.SentDate).ToList();
                return messages;
            };
        }
        //Tenant Messages
        public static List<tbMessage> getTenantMessages(long? id)
        {
            DateTime addsevendays = DateTime.Now.AddDays(7);

            using (var dv = new dbrealestateEntities())
            {
                //   dv.Configuration.LazyLoadingEnabled = true;
                var tenant = dv.Tenants.Where(p => p.Tenant_Id == id).FirstOrDefault();
                var messages = dv.tbMessages.Where(p => p.emailto == tenant.Email).OrderByDescending(p => p.SentDate).ToList();
                return messages;
            };
        }


        public static Tenant getTenantDetail(long? tenantId)
        {
            using (var dv = new dbrealestateEntities())
            {
                var gtd = dv.Tenants.Where(p => p.Tenant_Id == tenantId).FirstOrDefault();
                return gtd;
            };

        }
        public static Tenant getTenantDataByEmail(string email)
        {
            using (var dv = new dbrealestateEntities())
            {
                var gtd = dv.Tenants.Where(p => p.Email == email).FirstOrDefault();
                return gtd;
            };

        }
        public static User getlandlordDataByEmail(string email)
        {
            using (var dv = new dbrealestateEntities())
            {
                var cnt1 = dv.Users.Where(p => p.Email == email).FirstOrDefault();
                return cnt1;
            };

        }

    }
}