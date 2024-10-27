//DOM variables
const rangeType = "#rangeType";
const customRange = "#customRange";
const yearRange = "#yearRange";
const monthRange = "#monthRange";
const submitDate = "#submitDateRange";
const startDate = "#startDate";
const endDate = "#endDate";
const year = "#yearSelect";
const monthYear = "#monthYearSelect";
const month = "#monthSelect";
const chartReports = "#ChartReports";
const currentDateRange = "#CurrentDateRange";

const rangeCustom = "custom";
const rangeYear = "year";
const rangeMonth = "month";
const all = "all";
$(document).ready(function () {
    $(rangeType).change(function () {
        let type = $(this).val();
        if (type === rangeCustom) {
            $(customRange).show();
            $(yearRange).hide();
            $(monthRange).hide();
        } else if (type === rangeYear) {
            $(customRange).hide();
            $(yearRange).show();
            $(monthRange).hide();
        } else if (type === rangeMonth) {
            $(customRange).hide();
            $(yearRange).hide();
            $(monthRange).show();
        } else if (type === all) {
            $(customRange).hide();
            $(yearRange).hide();
            $(monthRange).hide();
        }
    });

    $(submitDate).click(function () {
        let type = $(rangeType).val();
        let data = {};

        if (type === rangeCustom) {
            data.StartDate = $(startDate).val();
            data.EndDate = $(endDate).val();

            $(currentDateRange).html(`Current Selection - Start Date: ${$(startDate).val()} - End Date: ${$(endDate).val()}`)
        } else if (type === rangeYear) {
            data.Year = $(year).val();
            $(currentDateRange).html(`Current Selection - Year: ${$(year).val()}`)
        } else if (type === rangeMonth) {
            data.Year = $(monthYear).val();
            data.Month = $(month).val();
            $(currentDateRange).html(`Current Selection - Month: ${$(month + " option:selected").text()} - Year: ${$(monthYear).val() }`)
        } else {
            $(currentDateRange).html("");
        }

        data.RangeType = type;

        
        showLoadSpinner();
        $('#dateRangeModal').modal('hide');
        fetch('/Expenses/GetDateRangeCharts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
        .then(response => response.text())
        .then(html => { 
            hideLoadSpinner()
            $(chartReports).html("")
            $(chartReports).html(html)
        }).catch(error => {
            console.error("Error:", error);
            hideLoadSpinner();
        })
        

    });
});

// Function to show the spinner
function showLoadSpinner() {
    $('#loadingSpinner').show();
}

// Function to hide the spinner
function hideLoadSpinner() {
    $('#loadingSpinner').hide();
}