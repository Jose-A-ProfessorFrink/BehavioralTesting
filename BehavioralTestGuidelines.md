## Introduction

In this article we will try to define by example the principles and patterns we use in behavioral testing. Since the goal of our testing strategy to maximize the amount of testing coverage we get through behavioral testing - understanding how to design, write and maintain these tests is the most important testing skill for you to develop. Before we get started, you will want to make sure you have this repository cloned.

```
https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/
```

You will need one of the following (vsCode is free):
- [Visual Studio 2022 (or higher)](https://visualstudio.microsoft.com/vs/)
- [VsCode 1.82.2 (or higher)](https://code.visualstudio.com/)

Optionally, you might also want the following
- [Postman](https://www.postman.com/)

## Description

The application we are working with is a simple internal company ordering site. We have customers, movies and orders. The customers come from our custom database which is this example is a simple liteDB file no sql document database. This database will be initiallized with random seed data the first time you run your application. Additionally, we rely on two free APIs as external data sources: one for movie information and another for zipcode information.

## Motivation

We wish to have a set of tests that accomplish the following objectives:
- Document the high level business behavior of the system in an easy to understand set of english sentence like specifications. These should be easily convertible from standard acceptance criteria in a story.
- Provide a very high level of coverage for all of our business rules and domain.
- Allow easy _refactor_ of internal implementations with no/minimal changes to tests themselves.
- Be as fast(or close to it) as standard [solitary unit tests](https://martinfowler.com/bliki/UnitTest.html#SolitaryOrSociable)

## Application Architecture 

![Application Architecture](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystemArchitecture.jpg?raw=true)

Everything within the green box represents what will be covered by our tests. The red box represents external boundaries that will not be covered by behavioral tests at all. This new layer, what we call the 'Provider' layer represents the lowest level conveniently mockable seam in our system. In some cases, this may be the 3rd party and/or framework interface itself. In other cases, it will be a custom wrapper we create explicitly to facilitate easy mocking and testing. We should strive to use the lowest level mockable interface possible that doesn't cross a process boundary and is easy to test. If no such thing exists, we should build the SIMPLEST, skinniest interface to accomodate that need. [GuidProvider.cs](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystem.Domain/Providers/GuidProvider.cs) is a good example of this.

## Anatomy of a behavioral test specification file

All behavioral tests are found in the behavioral test project. You will also find that there is a single specification file per endpoint in the solution. Organizing the tests around the endpoints is natural and makes sense. You should never have a specification file that covers more than a single endpoint. You may have valid reasons for having more than one specification file for a single endpoint. All tests are written using [XUnit](https://xunit.net/) and [FluentAssertions](https://fluentassertions.com/). The file we will be inspecting for this example is located [here](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystem.Tests/Behavioral/Orders/AddOrderItemSpecification.cs). 

Within each specification file you should have the following things:
- A constructor which has the common setup code that will be run prior to EVERY test. XUnit will create an instance for every test so this is consistent with XUnit best practices.
- Mocks as class level variables which are to be instrumented with working valid defaults (happy path) in the constructor. These mocks are then verified as needed in individual scenarios.
- Data as class level variables that are bound to the mocks and initialized with valid working defaults (happy path) in the constructor. 
- A WebApplicationFactory that represents the system under test (SUT).  This must be disposed of using an `IDisposable` pattern.  This is wrapper around [Microsoft's Web Application Factory](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1). This is Microsoft's recommended approach to testing web applications and provides a simple approach to abstracting the testing and setup of tests. The wrapper just provides a bevy of helper extension methods and functionality to make testing easier. It also facilitates our top level testing approach. It runs a testserver that spins up its own HttpClient which we can then use to execute requests against just as if it were a real API call. 
- A Default request. The reason we want to do this is we might expect data synchronicity between the default values and the incoming request. Although this is not a strict requirement, it simplifies writing your test scenarios.
- Specification definitions (the actual tests) that have a domain readable (avoid technical jargon where possible) behavior they are testing. 

### Constructor

The constructor is where most of the complexity of the specification file will live. Inside the constructor we will:
1. Create an application builder (using the same code that the production application will use) that leverages the Microsoft TestServer and WebApplicationFactory.
2. Mock out any wire dependencies that this particular code path may need (note that not you only need to mock out dependencies that your code will traverse. It is not required to mock out every _possible_ dependency). It IS required to mock out any other dependency causes us to cross a process boundary even ones we are not using. Often, for sad reasons, people/libraries do work in constructors (which is a bad practice BTW) and thus force us to mock out even dependencies we are not directly using. 
3. Every mocked out dependency should be stored in a class level variable. This is important because the individual tests will need access to the mock in order to verify that calls were made and, in some exceptional cases, re-instrument the mock with new behavior. The naming convention should be the name of the thing being mocked with a 'Mock' suffix. The 'Mock' suffix is important vs a 'Mock' prefix because it makes intellisense more useful. 
4. In most cases the mocked function will have a return value which the code will rely on. In this case, you will also want to setup a _working 'happy path' default value_ for the mock to return for its expected calls. This default value should be stored in a class level variable so it can be manipulated to hold different values for different test scenarios. Additionally, the value should be bound using a _lambda_ not directly. By using a lambda for the value to be returned the binding is deferred until execution which allows us more flexibility in changing the value in the setup (to null for example) without having to 'rebind' the mock in the test. 

#### Example

Below is a snippet of a constructor for the add order item tests. You should be able to identity all the above items in this snippet. Give it a try.

```CSharp
/// <summary>
/// This is a setup that is common for all scenarios in this specification class. Its responsibilities
/// include mocking out all the wire dependencies and setting appropriate happy path defaults on the mocked
/// out wire dependencies.
/// </summary>
    public AddOrderItemSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create();

        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = _webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.GetOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => _orderDataModel);
        _liteDbProviderMock
            .Setup(a=> a.UpdateOrderAsync(It.IsAny<OrderDataModel>()))
            .ReturnsAsync(() => _updateResult);

        // given I mock out the movie provider and setup appropriate defaults
        _movieProviderMock = _webApplicationFactory.Mock<IMovieProvider>();
        _movieProviderMock
            .Setup(a=>a.GetMovieAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(() => _getMovieApiResponse);

        // given I mock out the datetime provider and setup valid defaults
        _dateTimeProviderMock = _webApplicationFactory.Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(a=> a.UtcNow())
            .Returns(() => Defaults.UtcNow);
    }
```

Lets dig in a little deeper into step 4 of the constructor setup, since there are quite a few things to note. here is a typical example of setting up a mock with a default value:

```CSharp
(...)

private GetMovieApiResponse _getMovieApiResponse = new()
{
    ImdbId = Defaults.MovieId,
    Title= "Blade Runner 2049",
    Year= "2017",
    Rated= "R",
    Released= "06 Oct 2017",
    Plot= "a plot",
    Poster= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg",
    Metascore= "81",
    ImdbRating= "8.0",
    Type= "movie",
    Response= "True",
    Error = default
}; 

(... in the ctor)

// given I mock out the movie provider and setup appropriate defaults
_movieProviderMock = _webApplicationFactory.Mock<IMovieProvider>();
_movieProviderMock
    .Setup(a=>a.GetMovieAsync(It.IsAny<string>(), It.IsAny<string>()))
    .ReturnsAsync(() => _getMovieApiResponse);

(...)
```

In order to reduce the setup cost of all the tests in this specification file - you should provide a valid 'happy case' default value. If you do not do this, every test will need to incur that cost (regardless of whether or not that is something the specific test cares about). By providing a happy path default value for every mocked dependency, writing a test becomes as simple as manipulating a variable to produce a different result and then changing our verification. In this case, our default value is a movie called 'Blade Runner 2049' with defaults for all its properties.

The specific function we need on this mock 'GetMovieAsync' is instrumented to return this default value ANY time it is called. The use if It.IsAny<string>() here is NOT an oversight or random act of laziness. This is very deliberate to keep the setup as _flexible_ as possible since it will need to service many tests. Always use It.IsAny<> for all parameters of the setup function in your constructor. Although MOQ allows you to expect specific values, you should never do this here in the constructor. You should instead defer this responsibility to each test so it can verify its expectations. Not only is this simpler, its cleaner. Do no mix the concerns of valid defaults with verification. 

The last important trick we see here is in the 'Returns(' line. We use a lambda to return the default value instead of just feeding the value in directly. The lambda gives us _deferred_ resolution of that variable which is very useful (more examples will be provided) in certain cases and saves us from having to re-instrument the MOQ. The most common case where this trick is useful is if you need to have the _getMovieApiResponse be null. Without this deferred behavior, you will have to do a new setup on the MOQ. In general, deferred binding is something we ALWAYS strive for in tests. It gives us maximum flexibility.

With everything in place, our constructor code has setup our tests with the best possible scenario. In fact, if we have done it right, at this point we should be able to simply call the SUT in our test and expect the result to be successful.

###Specification (the test)
With all our setup in place, we should be ready to write our test cases. But first, lets take a moment to analyze a typical behavioral test. Please review the simple negative test case below.

```CSharp
[Fact(DisplayName = "Add order item should return bad request when order is not found")]
public async Task Test2()
{
    // given the order is not found
    _orderDataModel = default;

    // when I add an order item
    var response = await AddOrderItemAsync(_orderId, _request);

    // the I expect the response to be the following error
    await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
        "OrderIdInvalid",
        "The supplied order id is invalid");
}
```

####_Behavioral Test Naming_
The first thing that should jump out at you is how descriptive the test name is. The name of the test is arguably one of the most important features of the test. It is written in very plain english and communicates an expected business behavior of the specific endpoint. Even without knowing much about 'coding', someone with elementary knowledge of our domain would quickly be able to understand what this test is trying to enshrine. When naming a test, the test should always follow this naming pattern:

`<system> should <expected behavior> when <discriminating criteria>`

- The <system> element here is usually always the feature we are testing. In this case, we call it 'Create order' which refers to the create order endpoint in our service. For API behavioral tests, this will always represent an endpoint. So, if it helps, in this specific API project, you could rewrite the rule above to be:
`<endpoint> should <expected behavior> when <discriminating criteria>`
- the system is **_ALWAYS_** followed by the word 'should'. This convention makes all our tests easy to read and understand across repositories. If it helps, think of 'should' as a keyword for writing behavioral tests. 
- The next section describes the expected behavior. In this case it is 'return bad request'. This is clearly indicating what the test should expect for this given scenario. This statement should always be a non-ambiguous finite expectation. 

`Avoid catch all phrases like 'do all the things' or 'git er done'. When a tests expectations are not clear or overly broad, future maintainers may not be able to discern what was supposed to be tested. As a result, they may modify the test to omit verifying some behavior that was only being verified in this test. Ambiguity in tests is about as helpful as ambiguity in code. Try to explicitly enumerate the things you are actually testing. This will protect the intent and scope of the test from future degradation. If you feel the phrase is too generic, err on the side of being explicit. Additionally, avoid being lazy and adding additional assertions to existing tests, especially if they are not logically related. If a single test is getting too unwieldy or large, consider breaking it into many tests. When it doubt, break it out.`

`Avoid long 'and' chains when possible. For example, consider the following: 'Grandma should buy me a cake and a pony and tickets to the ball game and seven lollipops when it is my birthday'. In this case, the many validations may clutter the test. Consider breaking out the test into cohesive business behavioral expectations. This is NOT to say that EVERY test should have a single assertion. The high level behavior being described will almost guarantee that this is not the case. What it means is that a tests assertions should be cohesive. Again, as a last resort - when it doubt, break it out.` 

- The final bit of a test is the 'when <discriminating criteria>' part of the test. Please note that this part of the test is optional. Not all test cases will have discriminating criteria. It is perfectly legal to have a test without it. For example, I think we could all agree that the following test is perfectly valid: 'My boss should give me a raise'. If, however, you have a situation where the expectation is predicated on some other criteria, then that should always be included at the end of the test and specified by a 'when' keyword. Make sure you use 'when' as it keeps tests consistent. Your boss may insist you modify the test to be: 'My boss should give me a raise when I exceed expectations on my annual review and all my yearly projects were completed and the company has enough money'. Note how more than one discriminating criteria is chained with an 'and' keyword. Make sure you follow this pattern as well. Also note that you can have a long chain here. It is not as bad to have long chains in this part of the test, but try to keep it as short as possible. Again, if this becomes unwieldy, consider breaking it out into separate tests.

####_Behavioral test method body_

We should divide the test into three sections: Given, when and then. The given section should include statements that do necessary setup on the class level variables for the given behavior we expect. In this case, our test has one given statement that sets up the test to not find an order by manipulating our class level variable for the order mock (also note that in this specific case our code only works because we used a lambda to do deferred binding on the response for the mock). A test can contain zero or more given statements. Each statement should have an english like descriptive comment above it to help give an english-like flow to reading the scenario. Every test should contain one and ONLY one 'when' clause. This is a **_hard_** rule. There are no exceptions, anytime anywhere in the universe. The when should describe the action being invoked on the SUT. This will be the same within a given specification file for all the tests. Finally, the test should have a list of 1 or more 'then' assertions. It is important to note that there always should be at least ONE assertion. The assertions should each be described with an english like comment 'then ...'. The assertions should be clean and easy to read. 

####_Behavioral test request and response objects_

You may have noticed that the type we use for the request in this specification file is a bit odd. The declaration code is listed below:

```CSharp
private TestAddOrderItemRequestViewModel _request = new()
{
    MovieId = Defaults.MovieId,
    Quantity = 20
};
```

If you inspect the solution, you will see that this [file](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystem.Tests/TestModels/TestAddOrderItemRequestViewModel.cs) is defined in the test project. You may also notice that this file is eerily similar to the file that is [defined in the actual API project](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystem/ViewModels/AddOrderItemRequestViewModel.cs). Why have we done this? One principle we try to follow is we do not use the SUT to validate the SUT. In the end, our contract with the outside world is JSON not actual C# objects. By definining these test classes as separate from the actual objects the production code uses, we avoid the dreaded 'unwitting refactor/rename contract change' breaking our service by changing an external contract. This can occur when a developer renames a viewmodel property. Always create a dedicated test class for every viewmodel. Make sure you name this class 'Test<ClassName>'. This convention makes it easy to distinguish our test classes from the actual view model classes the application serves up in production. 

`If you have an enum in the viewmodel, make sure in the testviewmodel you define this as a string. this will force you to check against the string which is the actual contract also protecting you from enum renames breaking contracts.`

##Behavioral test - a success ('happy path') scenario
Next, lets analyze a test where we are testing a success case. The following test is a good candidate:

```CSharp
[Fact(DisplayName = "Add order item should store new order in the database correctly and return the correct order data")]
public async Task Test90()
{
    // when I add an order item
    var response = await AddOrderItemAsync(_orderId, _request);

    // then I expect the response to be created and contain the following
    await response.ShouldBeOkWithResponseAsync(new TestOrderViewModel
    {
        Id = Defaults.OrderId,
        Status = "New",
        Type = "Shipped",
        CreatedDateTimeUtc = Defaults.DateCreated,
        CancelledDateTimeUtc = default,
        CompletedDateTimeUtc = default,
        Shipping = 5M,
        LineItemTotal = 243M,
        DiscountTotal = 24.30M,
        TotalCost = 223.70M,
        Customer = new()
        {
            Id = Defaults.CustomerId,
            Name = Defaults.CustomerName,
            DateOfBirth = Defaults.CustomerDateOfBirth,
            DateHired = Defaults.CustomerDateHired,
            AnnualSalary = Defaults.CustomerAnnualSalary
        },
        ShippingAddress = new()
        {
            Line1  = "1121 Ash Lane",
            Line2 = "Southwest",
            City  = "Beverly Hills",
            State  = "CA",
            ZipCode  = "90210"
        },
        Items = new()
        {
            new()
            {
                MovieId = Defaults.MovieId,
                MovieYear = "2017",
                MovieMetascore = "81",
                Quantity = 20,
                Price = 12.15M
            }
        },
        Discounts = new()
        {
            new()
            {
                PercentDiscount = .1M,
                Type = "LargeOrder"
            }           
        }   
    });

    // then I expect the database to have been called with the following
    _liteDbProviderMock
        .Verify(a=> a.UpdateOrderAsync(ItShould.Be(new OrderDataModel()
        {
            Id = Defaults.OrderId,
            Status = Domain.Models.OrderStatus.New,
            Type = Domain.Models.OrderType.Shipped,
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = default,
            Shipping = 5M,
            LineItemTotal = 243M,
            DiscountTotal = 24.30M,
            Customer = new()
            {
                Id = Defaults.CustomerId,
                Name = Defaults.CustomerName,
                DateOfBirth = Defaults.CustomerDateOfBirth,
                DateHired = Defaults.CustomerDateHired,
                AnnualSalary = Defaults.CustomerAnnualSalary
            },
            ShippingAddress = new()
            {
                Line1  = "1121 Ash Lane",
                Line2 = "Southwest",
                City  = "Beverly Hills",
                State  = "CA",
                ZipCode  = Defaults.ZipCode
            },
            Items = new()
            {
                new()
                {
                    MovieId = Defaults.MovieId,
                    MovieYear = "2017",
                    MovieMetascore = "81",
                    Quantity = 20,
                    Price = 12.15M
                }
            } ,
            Discounts = new()
            {
                new()
                {
                    PercentDiscount = .1M,
                    Type = Domain.Models.DiscountType.LargeOrder
                }           
            }          
        })), Times.Once());
}

```

In the above code the first thing that should strike you off the bat is that we have no 'given' steps. There is no setup for this test at all. This is by design and deliberate. Because all the appropriate low level 'wire' mocks have been mocked and setup with working default values, the code can simply flow to an expected happy path scenario. 

`What if you have multiple 'happy path' options for how you instrument and mock? Which should you pick? The short answer is there is no hard and fast rule but in general a good strategy is to pick that one which is most representative of the most common use case. Testing is subject to the same rules of economics that everything else is, which is to say it should be rationalizing and your approach should strive to get the best 'bang for your buck' as possible. In some cases, I have seen endpoints where there is a super high level if/else that forces the behavior down completely different paths. In this case, it might make sense to create two separate specification files for that same endpoint. The bottom line is this is an art which takes both skill and common sense.`

Because the setup is out of the way, when we read this test we are able to focus all our attention to the verification steps which are the meat of this test. A few things we should note here:
- Each verification step has a descriptive English comment
- The response is checked against a test view model object
- The asserts laboriously inline all the data they are validating. The asserts are simple and not ashamed of being verbose or repetitive. One complaint I often hear around this is that people want to reference the setup classes to do the validation or worse - write some common validation method. Although this can be done, avoid it if it adds a lot of complexity to understanding what is being validated. Err on the side of simplicity. One of the goals of these tests is to provide easy to read documentation on behavior. The more you obfuscate the validation, the more you hurt this goal. Tests should be damp, not dry. 

And this is pretty much it. This is what we have been fighting for all along. All the work we have done previously was to enable this watershed moment. You can think of these scenarios as each representing a specific ['kata'](https://en.wikipedia.org/wiki/Karate_kata) of your system. The tests within each scenario are the same kata with an ever so minor variation of steps.

##Behavioral test theories
In some cases you may have a series of tests that are so similar they vary only by input data. For these types of tests you might elect to use xUnit theories. When you run into these cases, there are a few additional standards we like to follow. Here is an example:

```CSharp
[Theory(DisplayName = "Add order item should calculate item price correctly when ")]
[InlineData("movie has no known released year", "", "100", 5)]
[InlineData("movie has invalid movie year", "hello", "100", 5)]
[InlineData("movie has no known metascore", "1984", "", 5)]
[InlineData("movie has invalid metascore", "1984", "hello", 5)]
[InlineData("movie was released before 1945", "1944", "100", 2)]
[InlineData("movie was released in 1945", "1945", "100", 2)]
[InlineData("movie was released in 1970", "1970", "100", 6)]
[InlineData("movie was released in 2000", "2000", "100", 12)]
[InlineData("movie was released in 2020", "2020", "100", 15)]
[InlineData("movie was released after 2020", "2021", "100", 15)]
[InlineData("movie metascore is 0", "2021", "0", 0)]
[InlineData("movie metascore is 50", "2021", "50", 7.5)]
[InlineData("movie metascore is 75", "2021", "75", 11.25)]
public async Task Test6(string _, string movieYear, string metascore, decimal expectedPrice)
{
    // given The movie being added has released year and metascore
    _getMovieApiResponse = _getMovieApiResponse with { Year = movieYear, Metascore = metascore};

    // when I add an order item
    var response = await AddOrderItemAsync(_orderId, _request);

    // then I expect the response to be OK
    response.ShouldBeOk();

    // then I expect the line item price for the movie to be
    var order = await response.ReadContentAsAsync<TestOrderViewModel>();
    order.Items.Single().Price.Should().Be(expectedPrice);
}
```

The structure of the test follows all the same rules for test naming with one key distinction: The criteria that sets each test apart from each other in this theory is giving a meaningful English description in the form of when criteria of the main test. In this way, we can understand the test by simply concatenating the display name of the theory with each respective inline datas first string parameter. So, for example, to understand the second case in this theory, we would say 'Add order item should calculate item price correctly when movie has no known metascore'. This is necessary because we _need_ to have a proper English description for the test itself. It is not good enough to rely on the naked parameter data. By convention, I always give this parameter the `_` value. This makes it clear it is a discard and it shows up nicely in the test output window.

The addition of the `_` will give warnings about an unused parameter. Until there is a better solution in place for `InlineData()` that accepts a test name we need to suppress the compiler warning (challenge to readers, you could add this as a feature request and PR to the XUnit library...).

Parameter Warning Suppression Option 1:
```CSharp
// given I disable parameter warnings for an unused field
_ = _ + "";
```

Parameter Warning Suppression Option 2:
Provide a compiler suppression pragma which can be included in a global suppression file for the test project. If the warnings are suppressed it won't be obvious if any of the other theory parameters are also unused so be very careful with this approach.

Even though we can do theories, let me be the first to say that we should probably avoid them in all but the most appropriate cases (which I will describe below). Part of the benefit of our setup steps is that our tests do NOT have a high setup cost. This diminishes one of the main benefits of consolidating test cases into a theory. We could easily write this test as 3 separate tests. If we did that, it would be hard to argue that those tests would not read easier. You will pay a 'complexity tax' with theories. Again, don't be 'cute' trying to squeeze every drop of reuse you can. It is not the best approach. Ok, so with all that being said - you might rightly ask 'why is this a theory at all then?' It is a fair question, and my answer here would be because the functionality being tested is very cohesive within this domain. Since the price calculations just vary a bit, it actually makes more sense (or at least AS much) from a documentation point of view to put the tests together. Note this argument has NOTHING to do with code reuse and everything to do with the readability and documentation value of the tests. That should always be one of your most primary concerns. 

## Final thoughts: The right tool for the job

Behavioral tests are great for capturing high level business requirements. Often, we can capture all our functional requirements rather easily using this style of testing and generate fantastic documentation along the way. However, there are situations where some other form of unit testing makes more sense. Heavily algorithmic code might be an example of this. Something like a US State code validation algorithm might benefit more from plain old Solitary Unit testing. In that case, you might opt for a simple unit test and that may even be a better solution for maintainability. While I do not agree with everything [this page](https://www.practitest.com/resource-center/article/black-box-vs-white-box-testing/) is saying, I do agree with the point they make about using white box testing for heavy algorithm testing. 
