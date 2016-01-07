using System;
using System.Collections.Generic;
using System.Linq;

using BLL.Interfaces;

using Contracts;

using QuestradeAPI;

namespace BLL
{
	public class QuestradeService
	{
		private readonly IQuestradeApiTokenManager _tokenManager;
		private readonly InMemorySecurityRepository _securityRepo;

		public QuestradeService(IQuestradeApiTokenManager tokenManager, InMemorySecurityRepository securityRepository)
		{
			_tokenManager = tokenManager;
			_securityRepo = securityRepository;
		}

		public List<Account> GetAccounts()
		{
			var response = GetAccountsResponse.GetAccounts(_tokenManager.GetAuthToken());
			return response.Accounts.Select(a => new Account { Name = a.m_number }).ToList();
		}

		public List<Position> GetPositions(Account account)
		{
			var response = GetPositionsResponse.GetPositions(_tokenManager.GetAuthToken(), account.Name);
			return response.Positions.Select(qp => new Position
			{
				Account = account,
				Security = _securityRepo.GetBySymbol(qp.m_symbol),
				Shares = Convert.ToDecimal(qp.m_openQuantity)
			}).ToList();
		}
	}
}