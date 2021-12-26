﻿using Banks.MyDateTime;
using Banks.Tools.AccountException;

namespace Banks.BankSystem.Impl.StatesAccount.StatesDepositAccount
{
    public class DoubtfulDeposit : StateDeposit
    {
        internal override void CheckWithdrawal(float money)
        {
            if (Account.Info.LimitDoubtfulAccount < money ||
                Account.DepositEndDate > CurrentDate.GetInstance().Date ||
                money < 0)
                throw new TransactionCannotBeMade();
        }

        internal override void CheckReplenishment(float money)
        {
            if (Account.Info.LimitDoubtfulAccount < money || money < 0)
                throw new TransactionCannotBeMade();
        }

        internal override void Withdrawal(float money)
        {
            CheckWithdrawal(money);
            Account.Money -= money;
        }

        internal override void Replenishment(float money)
        {
            CheckReplenishment(money);
            Account.Money += money;
        }
    }
}