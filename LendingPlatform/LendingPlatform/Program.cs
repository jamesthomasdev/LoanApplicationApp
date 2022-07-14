
using LendingPlatform.Models;

public static class LendingApp
{
    /// <summary>
    /// A temporary recording of applications submitted during a session, in the absence of a persistent data solution
    /// </summary>
    private static List<Application> AllApplications = new List<Application>();

    public static void Main(string[] args)
    {
        while (true)
        {
            ShowAnalytics();

            GetNewApplication();

            // Start the loop over
        }
    }

    /// <summary>
    /// Gathers information from user input to create and submit a new loan application for consideration through the business logic
    /// </summary>
    public static void GetNewApplication()
    {
        // Start an application, and add it to the "data context"
        Application application = new Application();
        AllApplications.Add(application);

        // Get new application info
        Console.WriteLine("");
        Console.WriteLine("NEW LOAN APPLICATION:");

        while (application.BorrowingAmount <= 0)
        {
            Console.WriteLine("");
            Console.WriteLine("How much would you like to borrow? (Value must be greater than zero)");
            try
            {
                string input = Console.ReadLine();
                double borrowingAmount = double.Parse(input);

                application.BorrowingAmount = borrowingAmount;
            }
            catch
            {
                Console.WriteLine("Not a valid number");
            }
        }

        while (application.AssetValue <= 0)
        {
            Console.WriteLine("");
            Console.WriteLine("What is the total value of your asset against which to secure the loan? (Value must be greater than zero)");
            try
            {
                string input = Console.ReadLine();
                double assetVal = double.Parse(input);

                application.AssetValue = assetVal;
            }
            catch
            {
                Console.WriteLine("Not a valid number");
            }
        }

        // In the absence of a credit check facility which would be available to a production release, invite the user to provide their credit score
        while (application.CreditScore < 1)
        {
            Console.WriteLine("");
            Console.WriteLine("What is your credit score? (1-999)");
            try
            {
                string input = Console.ReadLine();
                int score = int.Parse(input);

                application.CreditScore = score;
            }
            catch
            {
                Console.WriteLine("Not a valid number");
            }
        }

        // Get a decision
        GetDecision(application);

        // Feedback on the decision
        Console.WriteLine("");
        Console.WriteLine($"Application decision: {application.Decision}");
        Console.WriteLine(application.DecisionText);
        Console.WriteLine("");
    }

    /// <summary>
    /// Checks the loan application against all business lending critera and updates the decision of the application accordingly
    /// </summary>
    /// <param name="application">The loan application being submitted for consideration</param>
    public static void GetDecision(Application application)
    {
        application.SubmittedAt = DateTime.Now;

        // Does the requested borrowing amount meet the business limitations?
        if (!CheckBorrowingAllowedRange(application.BorrowingAmount))
        {
            application.Decision = Definitions.EDecisionStatus.Rejected;
            application.DecisionText = $"The borrowing amount of {application.BorrowingAmount} does not fall within range of the business lending criteria.";
        }
        else
        {
            // Check the credit score based on the borrowing amount and the LTV of the application
            bool finalDecision = CheckCreditAndValuesForApplicant(application);
            if (!finalDecision)
            {
                application.Decision = Definitions.EDecisionStatus.Rejected;
                application.DecisionText = $"Application has been rejected because the an LTV of {application.LTVPercentage}% when borrowing {application.BorrowingAmount} with a credit score of {application.CreditScore} does not meet the lending criteria.";
            }
            else
            {
                application.Decision = Definitions.EDecisionStatus.Accepted;
                application.DecisionText = $"Congratulations, your application has been accepted.";
            }
        }
    }

    /// <summary>
    /// Checks the application against the business lending criteria based on the current credit score, amount to borrow, and secured asset value
    /// </summary>
    /// <param name="application">The loan application being submitted for consideration</param>
    /// <returns>Whether or not the application meets the credit score and values lending criteria as a bool</returns>
    private static bool CheckCreditAndValuesForApplicant(Application application)
    {
        if (application.BorrowingAmount >= 1000000)
        {
            return application.CreditScore >= 950 && application.LTVPercentage <= 60;
        }
        else
        {
            switch (application.LTVPercentage)
            {
                case < 60:
                    return application.CreditScore >= 750;
                case < 80:
                    return application.CreditScore >= 800;
                case < 90:
                    return application.CreditScore >= 900;
                default: // Values above 90% of LTV under these circumstances must be rejected
                    return false;
            }
        }
    }

    /// <summary>
    /// Compares the requested borrowing amount against the business lending allowed range and provides a decision as to whethere the criteria is satisfied
    /// </summary>
    /// <param name="borrowingAmount">The requested borrowing amount</param>
    /// <returns>Whether or not the borrwing amount is within the allowed lending range as a bool</returns>
    private static bool CheckBorrowingAllowedRange(double borrowingAmount)
    {
        // A full product may store these figures in a database or otherwise retrieve them from outside of the method
        double min = 100000;
        double max = 1500000;
        return borrowingAmount >= min && borrowingAmount <= max;
    }

    /// <summary>
    /// Displays a summary of useful analytics to the console
    /// </summary>
    public static void ShowAnalytics()
    {
        // Show the total applicants to date, broken down by success status
        Console.WriteLine($"Total Applications: {AllApplications.Count(a => a.Complete)}");
        Console.WriteLine($"Successful Applications: {AllApplications.Count(a => a.Complete && a.Decision == Definitions.EDecisionStatus.Accepted)}");
        Console.WriteLine($"Rejected Applications: {AllApplications.Count(a => a.Complete && a.Decision == Definitions.EDecisionStatus.Rejected)}");

        // Show total value of loans written to date
        Console.WriteLine("");
        Console.WriteLine($"Total value of all loans written to date: {TotalValueOfLoans()}");

        // Show the mean average loan to value of all applications received to date
        Console.WriteLine("");
        Console.WriteLine($"Mean LTV of all applications received: {LTVMeanAverage()}%");
    }

    /// <summary>
    /// Gets all applications which have been successful, and returns the sum of their borrowing amounts
    /// </summary>
    /// <returns>The sum of all borrowing amounts as a double</returns>
    private static double TotalValueOfLoans()
    {
        return AllApplications.Where(a => a.Complete && a.Decision == Definitions.EDecisionStatus.Accepted).Sum(a => a.BorrowingAmount);
    }

    /// <summary>
    /// Gets the sum of all LTV values across the applications so far, and returns their mean average
    /// </summary>
    /// <returns>The mean average of LTVs across all applications</returns>
    private static float LTVMeanAverage()
    {
        return AllApplications.Any(a => a.Complete) ? AllApplications.Where(a => a.Complete).Average(a => a.LTVPercentage) : 0;
    }
}