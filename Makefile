generate-allure-report:
	cp -R allure-reports/history allure-results
	rm -rf allure-reports
	allure generate allure-results -o allure-reports
	allure serve allure-results -p 10000

unit-tests:
	dotnet test --filter "FullyQualifiedName~UnitTests"

concat-reports:
	mkdir allure-results
	cp Tests/unit-allure-results/* allure-results/

.PHONY:
	generate-allure-report unit-tests concat-reports