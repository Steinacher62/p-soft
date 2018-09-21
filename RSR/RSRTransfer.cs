using ch.appl.psoft.db;
using ch.appl.psoft.Lohn;
using ch.psoft.Util;
using OGSSeek_srg;
using System;
using System.Collections;
namespace ch.appl.psoft.RSR
{
    public class RSRTransfer : Transfer
    {
        // Fields
        private bool _ageExists;
        private bool _costCenterDescriptionExists;
        private bool _costCenterNumberExists;
        private bool _employmentkindExists;
        private bool _employmenttypExists;
        private bool _funktionAbExists;
        private bool _funktionswertExists;
        private bool _repFunktionswertExists;
        private bool _repSalaryExists;
        private clsOGSSalary_srg _salary;
        private bool _superiorExists;

        // Methods
        public RSRTransfer()
        {
            this.initialize();
        }

        public RSRTransfer(DBData db)
            : base(db)
        {
            this.initialize();
        }

        protected override int EndLoadBudget()
        {
            return this._salary.EndLoadBudget();
        }

        protected override int EndLoadBudgetTyp()
        {
            return this._salary.EndLoadBudgetTyp();
        }

        protected override int EndLoadOrganisation()
        {
            return this._salary.EndLoadOrganisation();
        }

        protected override int EndLoadPerson()
        {
            return this._salary.EndLoadPerson();
        }

        protected override int EndLoadSalary()
        {
            return this._salary.EndLoadSalary();
        }

        protected override int EndLoadSalaryHistory()
        {
            return this._salary.EndLoadSalaryHistory();
        }

        protected override int EndStoreSalary()
        {
            return this._salary.EndStoreSalary();
        }

        protected override int EndTransfer()
        {
            return this._salary.EndTransfer();
        }

        protected override int GetBudget(string budgetRef, ref string organisationRef, ref double budget, ref int component, ref double budgetVerteilbar, ref string budgettypRef)
        {
            return this._salary.GetBudget(budgetRef, ref organisationRef, ref budget, ref component, ref budgetVerteilbar, ref budgettypRef);
        }

        protected override int GetBudgetTyp(string budgettypRef, ref string budgettypDescription)
        {
            return this._salary.GetBudgetTyp(budgettypRef, ref budgettypDescription);
        }

        protected override string getComponent(int ogsCode)
        {
            if ((ogsCode < 1) && (ogsCode > 2))
            {
                return "";
            }
            return RsrModule.SalaryComponentList[ogsCode - 1];
        }

        protected override int GetEmployment(bool historyData, string salaryRef, ref string personRef, ref string organisationRef, ref int typ, ref string function, ref string functionRef, ref string repFunction, ref string repFunctionRef, ref int salaryYear, ref ArrayList valueList)
        {
            string str = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            int num = 0;
            string str6 = "";
            int num2 = 0;
            int num3 = 0;
            valueList = new ArrayList();
            if (historyData)
            {
                num2 = this._salary.GetEmploymentPrev(salaryRef, ref personRef, ref organisationRef, ref typ, ref str, ref str2, ref str3, ref str4, ref str5, ref function, ref functionRef, ref num, ref str6, ref repFunction, ref repFunctionRef, ref num3, ref salaryYear);
            }
            else
            {
                num2 = this._salary.GetEmployment(salaryRef, ref personRef, ref organisationRef, ref typ, ref str, ref str2, ref str3, ref str4, ref str5, ref function, ref functionRef, ref num, ref str6, ref repFunction, ref repFunctionRef, ref num3, ref salaryYear);
            }
            if (this._employmenttypExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "EMPLOYMENTTYP", str));
            }
            if (this._employmentkindExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "EMPLOYMENTKIND", str2));
            }
            if (this._costCenterNumberExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "COSTCENTER_NUMBER", str3));
            }
            if (this._costCenterDescriptionExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "COSTCENTER_DESCRIPTION", str4));
            }
            if (this._superiorExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "SUPERIOR", str5));
            }
            if (this._funktionswertExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "FUNKTIONSWERT", num));
            }
            if (this._funktionAbExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "FUNKTION_AB", str6));
            }
            if (this._repFunktionswertExists)
            {
                valueList.Add(new UserAtributeValue("EMPLOYMENT", "REPFUNKTIONSWERT", num3));
            }
            return num2;
        }

        protected override string GetError(int errorCode)
        {
            return this._salary.GetError(errorCode);
        }

        protected override int GetNext(ref string reference)
        {
            return this._salary.GetNext(ref reference);
        }

        protected override int getOGSComponent(string component)
        {
            if (component == this.getComponent(1))
            {
                return 1;
            }
            if (component == this.getComponent(2))
            {
                return 2;
            }
            return -1;
        }

        protected override int GetOrganisation(string organisationRef, ref string parentRef, ref int nodelayout, ref string name, ref string mnemonic, ref int ordnummer)
        {
            return this._salary.GetOrganisation(organisationRef, ref parentRef, ref nodelayout, ref name, ref mnemonic, ref ordnummer);
        }

        protected override int GetPerson(string personRef, ref string initials, ref string password, ref string name, ref string firstname, ref string dateofbirth, ref string email, ref string title, ref string phone_intern, ref string mobile, ref string personalnumber, ref string entryDate, ref string geschlecht, ref int typ, ref ArrayList valueList)
        {
            int num = 0;
            valueList = new ArrayList();
            int num2 = this._salary.GetPerson(personRef, ref initials, ref password, ref name, ref firstname, ref dateofbirth, ref email, ref title, ref phone_intern, ref mobile, ref personalnumber, ref num, ref entryDate, ref geschlecht, ref typ);
            if (this._ageExists)
            {
                valueList.Add(new UserAtributeValue("PERSON", "AGE", num));
            }
            return num2;
        }

        protected override int GetSalary(bool history, string salaryRef, ref string personRef, ref string organisationRef, ref double occupation, ref int hourYear, ref int dayYear, ref int monthYear, ref int salaryKind, ref double currentSalary, ref double proposedSalary, ref double newSalary, ref int salaryYear, ref string employmentOrganisationRef, ref string budgettypRef, ref Hashtable rightTable, ref Hashtable componentValueTable, ref ArrayList valueList, ref Hashtable componentDescrTable, ref Hashtable componentInfoTable)
        {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            int num4 = 0;
            int num5 = 0;
            double num6 = 0.0;
            int num7 = 0;
            string str = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            double num8 = 0.0;
            valueList = new ArrayList();
            if (history)
            {
                num7 = this._salary.getSalaryPrev(salaryRef, ref personRef, ref organisationRef, ref occupation, ref hourYear, ref dayYear, ref monthYear, ref num, ref salaryKind, ref currentSalary, ref proposedSalary, ref newSalary, ref num2, ref str, ref num4, ref num5, ref num3, ref salaryYear, ref employmentOrganisationRef, ref budgettypRef, ref num6, ref str2, ref str3, ref str4, ref num8);
            }
            else
            {
                num7 = this._salary.GetSalary(salaryRef, ref personRef, ref organisationRef, ref occupation, ref hourYear, ref dayYear, ref monthYear, ref num, ref salaryKind, ref currentSalary, ref proposedSalary, ref newSalary, ref num2, ref str, ref num4, ref num5, ref num3, ref salaryYear, ref employmentOrganisationRef, ref budgettypRef, ref num6, ref str2, ref str3, ref str4, ref num8);
            }
            if (this._repSalaryExists)
            {
                valueList.Add(new UserAtributeValue("LOHN", "REPRICHTLOHN", num8));
            }
            rightTable = new Hashtable();
            rightTable.Add(this.ComponentList[0], (num4 == 1) && !history);
            rightTable.Add(this.ComponentList[1], (num5 == 1) || history);
            rightTable.Add(this.ComponentList[2], true);
            rightTable.Add(this.ComponentList[3], true);
            componentValueTable = new Hashtable();
            componentValueTable.Add(this.ComponentList[0], (double)(newSalary - currentSalary));
            componentValueTable.Add(this.ComponentList[1], num2);
            componentValueTable.Add(this.ComponentList[2], num3);
            componentValueTable.Add(this.ComponentList[3], num);
            componentDescrTable = new Hashtable();
            componentDescrTable.Add(this.ComponentList[0], str);
            componentDescrTable.Add(this.ComponentList[1], str2);
            componentInfoTable = new Hashtable();
            componentInfoTable.Add(this.ComponentList[0], str3);
            componentInfoTable.Add(this.ComponentList[1], str4);
            return num7;
        }

        private void initialize()
        {
            try
            {
                this._salary = new clsOGSSalary_srgClass();
                this._ageExists = base.tableColumnExists("PERSON", "AGE");
                this._employmenttypExists = base.tableColumnExists("EMPLOYMENT", "EMPLOYMENTTYP");
                this._employmentkindExists = base.tableColumnExists("EMPLOYMENT", "EMPLOYMENTKIND");
                this._costCenterNumberExists = base.tableColumnExists("EMPLOYMENT", "COSTCENTER_NUMBER");
                this._costCenterDescriptionExists = base.tableColumnExists("EMPLOYMENT", "COSTCENTER_DESCRIPTION");
                this._superiorExists = base.tableColumnExists("EMPLOYMENT", "SUPERIOR");
                this._funktionswertExists = base.tableColumnExists("EMPLOYMENT", "FUNKTIONSWERT");
                this._funktionAbExists = base.tableColumnExists("EMPLOYMENT", "FUNKTION_AB");
                this._repFunktionswertExists = base.tableColumnExists("EMPLOYMENT", "REPFUNKTIONSWERT");
                this._repSalaryExists = base.tableColumnExists("LOHN", "REPRICHTLOHN");
            }
            catch (Exception exception)
            {
                Logger.Log(exception, Logger.ERROR);
            }
        }

        protected override int PutSalary(string salaryRef, double newSalary, string comment, int component)
        {
            return this._salary.PutSalary(salaryRef, newSalary, comment, component);
        }

        protected override int StartLoadBudget()
        {
            return this._salary.StartLoadBudget();
        }

        protected override int StartLoadBudgetTyp()
        {
            return this._salary.StartLoadBudgetTyp();
        }

        protected override int StartLoadOrganisation()
        {
            return this._salary.StartLoadOrganisation();
        }

        protected override int StartLoadPerson()
        {
            return this._salary.StartLoadPerson();
        }

        protected override int StartLoadSalary()
        {
            return this._salary.StartLoadSalary();
        }

        protected override int StartLoadSalaryHistory()
        {
            return this._salary.StartLoadSalaryHistory();
        }

        protected override int StartStoreSalary()
        {
            return this._salary.StartStoreSalary();
        }

        protected override int StartTransfer(string dbName)
        {
            return this._salary.StartTransfer(dbName);
        }

        // Properties
        public override string[] ComponentList
        {
            get
            {
                return RsrModule.SalaryComponentList;
            }
        }

        public override string[] EditableComponentList
        {
            get
            {
                return new string[] { RsrModule.SalaryComponentList[0], RsrModule.SalaryComponentList[1] };
            }
        }

        public override string[] SalaryComponentList
        {
            get
            {
                return new string[] { RsrModule.SalaryComponentList[0] };
            }
        }
    }
}