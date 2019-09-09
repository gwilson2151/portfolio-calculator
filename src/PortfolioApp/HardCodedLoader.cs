using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioSmarts.Domain;

namespace PortfolioSmarts.PortfolioApp
{
	internal class HardCodedLoader
	{
        internal const int SecurityCategoryId = 1;
        internal const int PortfolioCategoryId = 2;

        private const int StocksValueId = 1;
        private const int BondsValueId = 2;
        private const int CashValueId = 3;
        private const int CaEquityValueId = 4;
        private const int UsEquityValueId = 5;
        private const int IntlEquityValueId = 6;
        private const int pBondsValueId = 7;
        private const int PreferredsValueId = 8;
        private const int AltValueId = 9;

        private IEnumerable<Category> Categories;
        private IDictionary<string, IEnumerable<CategoryWeight>> Weights;

        public Task<IEnumerable<Category>> LoadCategories() {
            if (Categories == null) {
                var security = new Category() {
                    Id = SecurityCategoryId,
                    Name = "Security Type"
                };
                security.Values.Add(new CategoryValue() {
                    Id = BondsValueId,
                    Name = "Bond",
                    Category = security
                });
                security.Values.Add(new CategoryValue() {
                    Id = StocksValueId,
                    Name = "Stock",
                    Category = security
                });
                security.Values.Add(new CategoryValue() {
                    Id = CashValueId,
                    Name = "Cash",
                    Category = security
                });

                var bdPortfolio = new Category() {
                    Id = PortfolioCategoryId,
                    Name = "Balanced & Diversified Portfolio"
                };
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = CaEquityValueId,
                    Name = "Canadian Equity",
                    Category = bdPortfolio
                });
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = UsEquityValueId,
                    Name = "US Equity",
                    Category = bdPortfolio
                });
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = IntlEquityValueId,
                    Name = "Int'l Equity",
                    Category = bdPortfolio
                });
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = AltValueId,
                    Name = "Alternative Strategy",
                    Category = bdPortfolio
                });
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = pBondsValueId,
                    Name = "Bonds",
                    Category = bdPortfolio
                });
                bdPortfolio.Values.Add(new CategoryValue() {
                    Id = PreferredsValueId,
                    Name = "Preferreds",
                    Category = bdPortfolio
                });

                Categories = new[] {
                    security,
                    bdPortfolio
                };
            }

            return Task.FromResult(Categories);
        }

        public Task<IDictionary<string, IEnumerable<CategoryWeight>>> LoadWeights(IEnumerable<Category> categories) {
            if (Weights == null) {
                Weights = new Dictionary<string, IEnumerable<CategoryWeight>>();
                var securityId = 1;

                var security = categories.Where(c => c.Id == SecurityCategoryId).Single();
                var portfolio = categories.Where(c => c.Id == PortfolioCategoryId).Single();

                var stock = security.Values.Where(v => v.Id == StocksValueId).Single();
                var bond = security.Values.Where(v => v.Id == BondsValueId).Single();
                var cash = security.Values.Where(v => v.Id == CashValueId).Single();

                var canEquity = portfolio.Values.Where(v => v.Id == CaEquityValueId).Single();
                var usEquity = portfolio.Values.Where(v => v.Id == UsEquityValueId).Single();
                var intlEquity = portfolio.Values.Where(v => v.Id == IntlEquityValueId).Single();
                var altStrat = portfolio.Values.Where(v => v.Id == AltValueId).Single();
                var pBonds = portfolio.Values.Where(v => v.Id == pBondsValueId).Single();
                var preferreds = portfolio.Values.Where(v => v.Id == PreferredsValueId).Single();

                var XUS = new Security() {
                    Symbol = "XUS.TO",
                    Exchange = "TSX",
                    Id = securityId++
                };
                Weights[XUS.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XUS,
                        Value = usEquity,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XUS,
                        Value = stock,
                        Weight = 100M
                    }
                };

                var VEF = new Security() {
                    Symbol = "VEF.TO",
                    Exchange = "TSX",
                    Id = securityId++
                };
                Weights[VEF.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = VEF,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = VEF,
                        Value = intlEquity,
                        Weight = 100M
                    }
                };

                var VCE = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "VCE.TO"
                };
                Weights[VCE.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = VCE,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = VCE,
                        Value = canEquity,
                        Weight = 100M
                    }
                };

                var VDY = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "VDY.TO"
                };
                Weights[VDY.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = VDY,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = VDY,
                        Value = altStrat,
                        Weight = 100M
                    }
                };

                var ZDV = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "ZDV.TO"
                };
                Weights[ZDV.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = ZDV,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = ZDV,
                        Value = altStrat,
                        Weight = 100M
                    }
                };

                var XCS = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XCS.TO"
                };
                Weights[XCS.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XCS,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XCS,
                        Value = canEquity,
                        Weight = 100M
                    }
                };

                var XSP = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XSP.TO"
                };
                Weights[XSP.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XSP,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XSP,
                        Value = usEquity,
                        Weight = 100M
                    }
                };

                var XSU = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XSU.TO"
                };
                Weights[XSU.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XSU,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XSU,
                        Value = usEquity,
                        Weight = 100M
                    }
                };

                var VSB = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "VSB.TO"
                };
                Weights[VSB.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = VSB,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = VSB,
                        Value = pBonds,
                        Weight = 100M
                    }
                };

                var XSB = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XSB.TO"
                };
                Weights[XSB.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XSB,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XSB,
                        Value = pBonds,
                        Weight = 100M
                    }
                };

                var HPR = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "HPR.TO"
                };
                Weights[HPR.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = HPR,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = HPR,
                        Value = preferreds,
                        Weight = 100M
                    }
                };

                var CIE = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "CIE.TO"
                };
                Weights[CIE.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = CIE,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = CIE,
                        Value = intlEquity,
                        Weight = 100M
                    }
                };

                var VEE = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "VEE.TO"
                };
                Weights[VEE.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = VEE,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = VEE,
                        Value = intlEquity,
                        Weight = 100M
                    }
                };

                var XIU = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XIU.TO"
                };
                Weights[XIU.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XIU,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XIU,
                        Value = canEquity,
                        Weight = 100M
                    }
                };

                var XIN = new Security() {
                    Id = securityId++,
                    Exchange = "TSX",
                    Symbol = "XIN.TO"
                };
                Weights[XIN.Symbol] = new[] {
                    new CategoryWeight() {
                        Security = XIN,
                        Value = stock,
                        Weight = 100M
                    },
                    new CategoryWeight() {
                        Security = XIN,
                        Value = intlEquity,
                        Weight = 100M
                    }
                };
            }

            return Task.FromResult(Weights);
        }
	}
}
