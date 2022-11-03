﻿using AutoMapper;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;
using EADA.Core.Domain.Messages.Dto;

namespace EADA.Core.Domain;

public class AutoMapperCoreProfile : Profile
{
    public AutoMapperCoreProfile()
    {
        #region Expense

        CreateMap<Expense, ExpenseDto>();

        CreateMap<ExpenseArgs, Expense>()
            .ForMember(x => x.ExpenseId, map => map.Condition(x => x.ExpenseId > 0));

        #endregion
    }
}