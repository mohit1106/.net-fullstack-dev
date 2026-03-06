using System;
using System.Collections.Generic;
using System.Text;

namespace FacadePattern
{
    public class IceCreamFacade
    {
        private FlavorService _flavorService;
        private ConeService _coneService;
        private PaymentService _paymentService;

        public IceCreamFacade()
        {
            _flavorService = new FlavorService();
            _coneService = new ConeService();
            _paymentService = new PaymentService();
        }

        public string GetIceCream()
        {
            Console.WriteLine("Mother is arranging the ice cream...");

            string flavor = _flavorService.GetFlavor();
            string cone = _coneService.GetCone();

            _paymentService.ProcessPayment();

            return flavor + " in a " + cone;
        }
    }
}


