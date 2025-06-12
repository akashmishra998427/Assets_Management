
function initalizeDatatable(ApiUrlMaster, tableid) {
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: "GET",
        dataType: "json",
        beforeSend: function () {
            $("#tableBody").append(`
              <tr>
                <td colspan="100%" class="text-center py-5">
                  <div class="d-flex justify-content-center align-items-center">
                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                      <span class="sr-only">Loading...</span>
                    </div>
                    <div class="ms-3 fs-5 text-muted">Please wait, loading data...</div>
                  </div>
                </td>
              </tr>
            `);
        },
        success: function (data) {
            if ($.fn.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $("#tableHeaders").empty();
            $("#tableBody").empty();
            if (!data || data.length == 0) {
                $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
                return;
            }
            let headers = Object.keys(data[0]);
            headers.forEach(header => {
                $("#tableHeaders").append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
            });
            data.forEach(item => {
                let row = `<tr class="rowHeight">`;
                headers.forEach(header => {
                    let cellData = item[header] !== null && item[header] !== undefined ? item[header] : '';
                    row += `<td>${cellData}</td>`;
                });
                row += `</tr>`;
                $("#tableBody").append(row);
            });
            var table = ID.DataTable({
                paging: false,
                searching: true,
                ordering: false,
                scrollCollapse: false,
                scrollY: "500px",
                info: true,
                autoWidth: false,
                fixedHeader: true,
            });
            ID.find("tbody").off("click").on("click", "tr", function () {
                var rowIndex = table.row(this).index();
                var rowData = data[rowIndex];
                let modalContent = "<ul class='list-group'>";
                Object.entries(rowData).forEach(([key, value]) => {
                    modalContent += `<li class='list-group-item'><strong>${key || ''}:</strong> ${value || ''}</li>`;
                });
                modalContent += "</ul>";
                $("#modalBodyContent").html(modalContent);
                $("#dataModal").modal("show");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
        }
    });
}

function DatatablePopulation(ApiUrlMaster, tableid, AuthToken, PayLoad, Method) {
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: Method,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PayLoad),
        headers: { "Authorization": `Bearer ${AuthToken}` },
        success: function (data) {
            if ($.fn.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $("#tableHeaders").empty();
            $("#tableBody").empty();
            if (!data || data.length === 0) {
                $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
            }
            let headers = Object.keys(data[0]);
            headers.forEach(header => {
                $("#tableHeaders").append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
            });
            data.forEach(item => {
                let row = `<tr class="rowHeight">`;
                headers.forEach(header => {
                    let cellData = item[header] !== null && item[header] !== undefined ? item[header] : '';
                    row += `<td>${cellData}</td>`;
                });
                row += `</tr>`;
                $("#tableBody").append(row);
            });

            var table = ID.DataTable({
                paging: false,
                searching: true,
                ordering: true,
                scrollCollapse: false,
                scrollY: "500px",
                info: true,
                autoWidth: true,
                fixedHeader: true,
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            $("#tableBody").empty();
            $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
        }
    });
}

function DatatablePopulationwithFooter(ApiUrlMaster, tableid, AuthToken, PayLoad, Method, scrollY) {
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: Method,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PayLoad),
        headers: { "Authorization": `Bearer ${AuthToken}` },
        success: function (data) {
            if ($.fn.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $("#tableHeaders").empty();
            $("#tableBody").empty();
            $("#tableFooter").empty();
            if (!data || data.length === 0) {
                $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
                $("#tableFooter").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
                return;
            }
            const isOfficeRegReport = PayLoad.Fillter === "Office" && PayLoad.ReportType === "Reg";
            let headers;
            let sums = [];

            if (isOfficeRegReport) {
                const requiredColumns = ["DEPT", "Microsoft Office Standard 2007", "OpenOffice 4.1.1"];

                headers = requiredColumns.filter(col =>
                    Object.keys(data[0]).some(key => key === col || key.toUpperCase() === col.toUpperCase())
                );

                if (headers.length < 3) {
                    headers = Object.keys(data[0]).slice(0, 3);
                }
            } else {
                headers = Object.keys(data[0]);
            }
            sums = new Array(headers.length).fill(0);
            headers.forEach(header => {
                $("#tableHeaders").append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
            });

            data.forEach(item => {
                let row = `<tr class="rowHeight">`;
                headers.forEach((header, index) => {
                    let cellData = item[header] !== null && item[header] !== undefined ? item[header] : '';
                    if ($.isNumeric(cellData)) {
                        sums[index] += parseFloat(cellData);
                    }
                    row += `<td>${cellData}</td>`;
                });
                row += `</tr>`;
                $("#tableBody").append(row);
            });

            let footerRow = `<tr>`;
            footerRow += `<td class="fw-bold">Total</td>`;
            sums.forEach((sum, index) => {
                if (index > 0) {
                    footerRow += `<td>${sum.toFixed(0)}</td>`;
                }
            });
            footerRow += `</tr>`;
            $("#tableFooter").append(footerRow);

            var table = ID.DataTable({
                paging: false,
                searching: true,
                ordering: true,
                scrollCollapse: false,
                scrollY: scrollY,
                info: true,
                autoWidth: true,
                fixedHeader: true,
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            $("#tableBody").empty();
            $("#tableBody").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
            $("#tableFooter").empty();
            $("#tableFooter").append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
        }
    });
}

function DatatablePopulationNew(ApiUrlMaster, tableid, AuthToken, PayLoad, Method, tableHeaders, tblBody) {
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: Method,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PayLoad),
        headers: { "Authorization": `Bearer ${AuthToken}` },
        beforeSend: function () {
            $(`#${tblBody}`).append(`
              <tr>
                <td colspan="100%" class="text-center py-5">
                  <div class="d-flex justify-content-center align-items-center">
                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                      <span class="sr-only">Loading...</span>
                    </div>
                    <div class="ms-3 fs-5 text-muted">Please wait, loading data...</div>
                  </div>
                </td>
              </tr>
            `);
        },
        success: function (data) {
            if ($.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $(`#${tableHeaders}`).empty();
            $(`#${tblBody}`).empty();
            if (!data || data.length === 0) {
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
            }
            else {
                let headers = Object.keys(data[0]);
                headers.forEach(header => {
                    $(`#${tableHeaders}`).append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
                });
                data.forEach(item => {
                    let row = `<tr class="rowHeight">`;
                    headers.forEach(header => {
                        let cellData = item[header] || '';
                        row += `<td>${cellData}</td>`;
                    });
                    row += `</tr>`;
                    $(`#${tblBody}`).append(row);
                });
                var table = ID.DataTable({
                    paging: true,
                    searching: true,
                    ordering: true,
                    scrollCollapse: false,
                    scrollY: "500px",
                    info: true,
                    autoWidth: false,
                    fixedHeader: true,
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            $(`#${tblBody}`).empty();
            $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
        },
        complete: function () {
        }
    });
}

//function ModalDataTableAssetdaetail(ApiUrlMaster, tableid, AuthToken, PayLoad, Method, tableHeaders, tblBody, Scroll) {
//    debugger
//    var ID = $(`#${tableid}`);
//    let URL = ApiUrlMaster;
//    $.ajax({
//        url: URL,
//        method: Method,
//        dataType: "json",
//        contentType: "application/json",
//        data: JSON.stringify(PayLoad),
//        headers: { "Authorization": `Bearer ${AuthToken}` },
//        beforeSend: function () {
//            $(`#${tblBody}`).append(`
//              <tr>
//                <td colspan="100%" class="text-center py-5">
//                  <div class="d-flex justify-content-center align-items-center">
//                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
//                      <span class="sr-only">Loading...</span>
//                    </div>
//                    <div class="ms-3 fs-5 text-muted">Please wait, loading data...</div>
//                  </div>
//                </td>
//              </tr>
//            `);
//        },
//        success: function (data) {
//            if ($.fn.DataTable.isDataTable(ID)) {
//                ID.DataTable().destroy();
//            }
//            $(`#${tableHeaders}`).empty();
//            $(`#${tblBody}`).empty();
//            if (!data || data.length === 0) {
//                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
//            }
//            else {
//                let headers = Object.keys(data[0]);
//                headers.forEach(header => {
//                    $(`#${tableHeaders}`).append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
//                });
//                data.forEach((item, index) => {
//                    let row = `<tr class="rowHeight" id="${index}">`;
//                    headers.forEach(header => {
//                        let cellData = item[header] || '';
//                        row += `<td>${cellData}</td>`;
//                    });
//                    row += `</tr>`;
//                    $(`#${tblBody}`).append(row);
//                });
//                var table = ID.DataTable({
//                    paging: false,
//                    searching: false,
//                    ordering: false,
//                    scrollCollapse: false,
//                    info: true,
//                    autoWidth: false,
//                    fixedHeader: false,
//                });
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching data:", error);
//            $(`#${tblBody}`).empty();
//            $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
//        },
//        complete: function () {
//        }
//    });
//}



function ModalDataTableAssetdaetail(ApiUrlMaster, tableid, AuthToken, PayLoad, Method, tableHeaders, tblBody, Scroll) {
    debugger
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: Method,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PayLoad),
        headers: { "Authorization": `Bearer ${AuthToken}` },
        beforeSend: function () {
            $(`#${tblBody}`).append(`
              <tr>
                <td colspan="100%" class="text-center py-5">
                  <div class="d-flex justify-content-center align-items-center">
                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                      <span class="sr-only">Loading...</span>
                    </div>
                    <div class="ms-3 fs-5 text-muted">Please wait, loading data...</div>
                  </div>
                </td>
              </tr>
            `);
        },
        success: function (data) {
            if ($.fn.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $(`#${tableHeaders}`).empty();
            $(`#${tblBody}`).empty();
            console.log("API Response:", data);
            if (!data) {
                console.log("No data received");
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
                return;
            }
            if (!Array.isArray(data)) {
                console.log("Data is not an array:", typeof data);
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">Invalid data format</td></tr>');
                return;
            }
            if (data.length === 0) {
                console.log("Data array is empty");
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
                return;
            }
            const firstItem = data[0];
            if (!firstItem || typeof firstItem !== 'object' || firstItem === null) {
                console.log("First item is invalid:", firstItem);
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">Invalid data structure</td></tr>');
                return;
            }
            let headers;
            try {
                headers = Object.keys(firstItem);
            } catch (error) {
                console.error("Error getting object keys:", error);
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">Error processing data structure</td></tr>');
                return;
            }
            if (headers.length === 0) {
                console.log("No properties found in first object");
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data structure available</td></tr>');
                return;
            }
            headers.forEach(header => {
                $(`#${tableHeaders}`).append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
            });
            data.forEach((item, index) => {
                if (!item || typeof item !== 'object') {
                    return;
                }
                let row = `<tr class="rowHeight" id="${index}">`;
                headers.forEach(header => {
                    let cellData = item[header] || '';
                    cellData = $('<div>').text(cellData).html();
                    row += `<td>${cellData}</td>`;
                });
                row += `</tr>`;
                $(`#${tblBody}`).append(row);
            });
            var table = ID.DataTable({
                paging: false,
                searching: false,
                ordering: false,
                scrollCollapse: false,
                info: true,
                autoWidth: false,
                fixedHeader: false,
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            console.error("Response:", xhr.responseText);
            $(`#${tblBody}`).empty();
            $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">Error loading data. Please try again.</td></tr>');
        },
        complete: function () {
        }
    });
}
function ModalDataTable(ApiUrlMaster, tableid, AuthToken, PayLoad, Method, tableHeaders, tblBody, Scroll) {
    var ID = $(`#${tableid}`);
    let URL = ApiUrlMaster;
    $.ajax({
        url: URL,
        method: Method,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(PayLoad),
        headers: { "Authorization": `Bearer ${AuthToken}` },
        beforeSend: function () {
            $(`#${tblBody}`).append(`
              <tr>
                <td colspan="100%" class="text-center py-5">
                  <div class="d-flex justify-content-center align-items-center">
                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                      <span class="sr-only">Loading...</span>
                    </div>
                    <div class="ms-3 fs-5 text-muted">Please wait, loading data...</div>
                  </div>
                </td>
              </tr>

            `);
        },
        success: function (data) {
            if ($.fn.DataTable.isDataTable(ID)) {
                ID.DataTable().destroy();
            }
            $(`#${tableHeaders}`).empty();
            $(`#${tblBody}`).empty();
            if (!data || data.length === 0) {
                $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
            }
            else {
                let headers = Object.keys(data[0]);
                headers.forEach(header => {
                    $(`#${tableHeaders}`).append(`<th>${header.replace(/\s+/g, ' ').trim()}</th>`);
                });
                data.forEach((item, index) => {
                    let row = `<tr class="rowHeight" id="${index}">`;
                    headers.forEach(header => {
                        let cellData = item[header] || '';
                        row += `<td>${cellData}</td>`;
                    });
                    row += `</tr>`;
                    $(`#${tblBody}`).append(row);
                });
                var table = ID.DataTable({
                    paging: true,
                    searching: true,
                    ordering: false,
                    scrollCollapse: false,
                    scrollY: Scroll,
                    info: true,
                    autoWidth: true,
                    fixedHeader: true,
                });
                setTimeout(function () {
                    table.columns.adjust().draw();
                }, 100);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            $(`#${tblBody}`).empty();
            $(`#${tblBody}`).append('<tr><td colspan="100%" class="text-center">No data available</td></tr>');
        },
        complete: function () {
        }
    });
}

function bindResions() {
    let Baseurl = "@ViewBag.Apiurl"
    let url = `${Baseurl}Stitch/GetRejectReason?Type=Machine&StyleId=0&Desc=`
    $.ajax({
        url: url,
        method: "GET",
        dataType: "json",
        success: function (data) {
            let ddlResion = $('#ddlresion');
            ddlResion.empty().append(`<option value="" selected class="fw-bold">Select Resions</option>`);
            data.forEach(item => {
                ddlResion.append(`<option class="fw-bold" value="${item.Reason}">${item.RemarkText.trim()}</option>`);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching premises:", error);
        }
    });
}

function showToast(message, type) {
    let title = type === "success" ? "Success" : "Error";
    let icon = type === "success" ? "success" : "error";
    Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: "OK",
        timer: 1500,
        showConfirmButton: false
    });
}

function SweetAlter(message, type) {
    let title = type === "success" ? "Success" :
        (type === "warning" ? "Warning" :
            (type === "info" ? "Information" : "Error"));
    let icon = type === "success" ? "success" :
        (type === "warning" ? "warning" :
            (type === "info" ? "info" : "error"));
    const swalConfig = {
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: "OK",
        timer: 2000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end',
        customClass: {
            popup: 'swal-on-top'
        }
    };
    Swal.fire(swalConfig);
}
const styleElement = document.createElement('style');
styleElement.textContent = `
    .swal-on-top {
        z-index: 9999 !important;
    }
    .swal2-container {
        z-index: 9999 !important;
    }
`;
document.head.appendChild(styleElement);

function showConfirmationDialog(header, message, icon, confirmButtonText, cancelButtonText, successMessage) {
    Swal.fire({
        title: header,
        text: message,
        icon: icon,
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: "Success!",
                text: successMessage,
                icon: "success"
            });
        }
    });
}

function exportToExcel(tableID, WorkbookName) {
    var table = document.getElementById(tableID);
    var Filename = WorkbookName;
    var wb = XLSX.utils.table_to_book(table, { sheet: "Sheet1" });
    XLSX.writeFile(wb, `${Filename}.xlsx`);
}

function initalizeDatatableFixedHeaders(URL, tableid, headers, rowItems) {
    axios.get(URL)
   .then(response => {
       const data = response.data;
       if (!data || data.length === 0) {
           $("#tableHeaders").empty();
           $("#tableBody").empty();
           return;
       }
       const ID = $(`#${tableid}`);
       if ($.fn.DataTable.isDataTable(ID)) {         
           ID.DataTable().destroy();
       }
       const tableHeader = $("#tableHeaders");
       tableHeader.empty();
       tableHeader.append(`<th>CPU_ASSET_CODE</th>`);
       tableHeader.append(`<th>Ip_No</th>`);
       tableHeader.append(`<th>DEPT</th>`);
       tableHeader.append(`<th>USER_NAME</th>`);
       tableHeader.append(`<th>StartDate</th>`);
       tableHeader.append(`<th>EndDate</th>`);
       tableHeader.append(`<th>TodayScan</th>`);
       tableHeader.append(`<th>Active</th>`);
       tableHeader.append(`<th>Exclude</th>`);
       tableHeader.append(`<th>Action</th>`);
       $("#tableBody").empty();
       data.forEach(item => {
           let row = `<tr class="rowHeight">`;
           row += `<td>${item.CPU_ASSET_CODE || ""}</td>`;
           row += `<td class="text-primary"><p id="ipno" onclick="DowenlodIpData('${item.CPU_ASSET_CODE}')">${item.Ip_No || ""}</p></td>`;
           row += `<td>${item.DEPT || ""}</td>`;
           row += `<td>${item.USER_NAME || ""}</td>`;
           row += `<td>${item.StartDate || ""}</td>`;
           row += `<td>${item.EndDate || ""}</td>`;
           row += `<td>${item.TodayScan || ""}</td>`;
           row += `<td>${item.Active || ""}</td>`;
           row += `<td>${item.Exclude || ""}</td>`;
           row += `<td>
                  <div class="d-flex gap-2">
                      <div class="openFormBtn" data-id="${item.CPU_ASSET_CODE}"
                          onclick="ConfermationSwwetAlert('Confirmation','Are you sure to Exclude PC ?', 'warning', 'Exclude', '#d33', '#3085d6','PC Exclude Successfylly', 'success','btnExclude',ActivitySetting(${1}))" 
                          style="margin-left:10px; color:#0A4057; font-weight:700; font-size:20px">
                          <i class="fa-solid fa-xmark"></i>
                      </div>

                      <div class="openFormBtn" data-id="${item.CPU_ASSET_CODE}"
                          onclick="ConfermationSwwetAlert('Confirmation',' Are you sure to Re-Scan ?', 'warning', ' Re-Scan', '#d33', '#3085d6', ' Re-Scan Successfylly', 'success','btnRe-Scan',ActivitySetting(${3}))" 
                          style="margin-left:10px; color:#0A4057; font-weight:700; font-size:20px">
                          <i class="fa-solid fa-rotate"></i>
                      </div>
   
                      <div class="openFormBtn" data-id="${item.CPU_ASSET_CODE}"
                          onclick="ConfermationSwwetAlert('Confirmation',' Are you sure to Stop Scanning ?', 'warning', 'Stop Scanning ', '#d33', '#3085d6', 'Scaning Stoped Successfylly', 'success','btnStop-Scanning',ActivitySetting(${4}))" 
                          style="margin-left:10px; color:#0A4057; font-weight:700; font-size:20px">
                          <i class="fa-solid fa-power-off"></i>
                      </div>
                  </div>
           </td>`;

           row += `</tr>`;
           $("#tableBody").append(row);
       });
       ID.DataTable({
           paging: false,
           searching: true,
           ordering: false,
           info: false,
           fixedHeader: true,
       });
   })
   .catch(error => {
       console.error("Error fetching data:", error);
   });
}
function DowenlodIpData(assetCode) {
    debugger
    if (assetCode) {
        let URL = `/Dashboard/DetailPage?AssetCode=${assetCode}`;
        $('#assetModal iframe').attr('src', URL);
        $('#assetModal').modal('show');
        $('#tbtDetailDetailModal').text(`Asset Details (${assetCode})`)
    }
}
function BindDropdown(apiUrl, dropdownId, authToken, valueField, textField, placeholder) {
    axios.get(apiUrl, { headers: { "Authorization": `Bearer ${authToken}` } })
        .then(function (response) {
            let dropdown = $(`#${dropdownId}`);
            dropdown.empty().append(`<option value="0">Select ${placeholder}</option>`);
            response.data.forEach(item => {
                let value = item[valueField] ? item[valueField].toString().trim() : "";
                let text = item[textField] ? item[textField].toString().trim() : "";
                dropdown.append(`<option value="${value}">${text ? text : "N/A"}</option>`);
            });
            dropdown.select2({
                placeholder: `Select ${placeholder}`,
                allowClear: true
            });
        })
        .catch(function (error) {
            console.error(`Error fetching data for ${dropdownId}:`, error);
        });
}

function BindCheckboxes(apiUrl, sectionId, authToken, valueField, textField, placeholder, className) {
    axios.get(apiUrl, { headers: { "Authorization": `Bearer ${authToken}` } })
        .then(function (response) {
            let section = $(`#${sectionId}`);
            section.empty();
            const data = response.data;
            if (!Array.isArray(data) || data.length === 0) {
                section.append(`<p>No ${placeholder} available.</p>`);
                return;
            }
            data.forEach(item => {
                let value = item[valueField] ? item[valueField].toString().trim() : "";
                let text = item[textField] ? item[textField].toString().trim() : "";

                section.append(`
            <div class="checkbox-container">
                <input type="checkbox" id="${value}" value="${value}" class="checkbox-option ${className}">
                <label for="${value}">${text || "N/A"}</label>
            </div>
        `);
            });

        })
        .catch(function (err) {
            console.error(`Error fetching data for ${sectionId}:`, err);
        });
}

function setupCheckAll(id, className) {
    $(id).on('change', function () {
        if ($(this).prop('checked')) {
            $(className).prop('checked', true);
        }
        else {
            $(className).prop('checked', false);
        }
    });

    $(className).on('change', function () {
        if ($(className).length === $(className + ':checked').length) {
            $(id).prop('checked', true);
        }
        else {
            $(id).prop('checked', false);
        }
    });
}

function showRowDetails(URL, sectionID, Method, authToken, payLoad) {
    let loaderHTML = `<div class="d-flex justify-content-center align-items-center mt-3">
                       <div class="spinner-border text-primary" role="status">
                           <span class="visually-hidden">Please Wait Loading...</span>
                       </div>
                     </div>`;
    document.getElementById(sectionID).innerHTML = loaderHTML;
    axios({
        url: URL,
        method: Method,
        headers: {
            'Authorization': 'Bearer ' + authToken,
            'Content-Type': 'application/json'
        },
        data: payLoad
    })
    .then(response => {
        const data = response.data;
        let tableHTML = `<table class="table table-hover" id="PoputDetails"><tbody>`;

        data.forEach(item => {
            const itemEntries = Object.entries(item);
            for (let i = 0; i < itemEntries.length; i += 2) {
                let key1 = itemEntries[i][0];
                let value1 = itemEntries[i][1] ?? " ";
                let key2 = itemEntries[i + 1] ? itemEntries[i + 1][0] : '';
                let value2 = itemEntries[i + 1] ? itemEntries[i + 1][1] : " ";
                tableHTML += `
                 <tr>
                     <td style="font-size:12px; font-weight:700">${key1}:</td>
                     <td style="font-size:12px; font-weight:500">${value1 || ""}</td>
                     <td style="font-size:12px; font-weight:700">${key2}:</td>
                     <td style="font-size:12px; font-weight:500">${value2 || ""}</td>
                  </tr>
            `;
            }
        });

        tableHTML += `</tbody></table>`;
        document.getElementById(sectionID).innerHTML = tableHTML;
    })
    .catch(error => {
        const errorMsg = error.response?.data || error.message;
        document.getElementById(sectionID).innerHTML = `<div class="alert alert-danger">Error loading data: ${errorMsg}</div>`;
    });
}

function getRowDataOnClick(tableid) {
    $(`#${tableid} tbody`).on('click', 'tr', function () {
        let rowId = $(this).attr('id');
        let rowData = [];
        $(this).find('td').each(function () {
            rowData.push($(this).text().trim());
        });
    });
}

function ConfermationSwwetAlert(title1, text1, icon1, confirmButtonText, confirmButtonColor, cancelButtonColor, text2, icon2, confirmButtonId, customMethod,) {
    Swal.fire({
        title: title1,
        text: text1,
        icon: icon1,
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: cancelButtonColor,
        confirmButtonText: confirmButtonText
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                text: text2,
                icon: icon2
            });
        }

    });
    const confirmButton = Swal.getConfirmButton();
    confirmButton.setAttribute('id', confirmButtonId);
    confirmButton.addEventListener('click', function () {
        customMethod()
    });
}

function ZoomerOPEN(e) {
    var path = $(e).attr("src");
    window.open(path);
}

function bindCheckboxOptions(url, textField, valueField, sectionId, chkDepartment) {
    $.ajax({
        url: url,
        method: "GET",
        success: function (response) {
            let section = $(`#${sectionId}`);
            section.empty();
            let data;
            try {
                data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;
            } catch (e) {
                console.error("Failed to parse data:", e);
                section.append(`<p>Error loading data.</p>`);
                return;
            }
            if (!Array.isArray(data) || data.length === 0) {
                section.append(`<p>No data available.</p>`);
                return;
            }
            data.forEach((item, index) => {
                let rawValue = item?.[valueField];
                let rawText = item?.[textField];

                let value = (rawValue !== null && rawValue !== undefined) ? rawValue.toString().trim() : `val_${index}`;
                let text = (rawText !== null && rawText !== undefined && rawText.toString().trim() !== "")
                    ? rawText.toString().trim()
                    : "N/A";
                section.append(`
                    <div class="checkbox-container">
                        <input type="checkbox" id="${value}" value="${value}" class="checkbox-option chkDynamic ${chkDepartment}">
                        <label for="${value}">${text}</label>
                    </div>
                `);
            });
            $(`#${sectionId} .chkDynamic`).prop('checked', true);
        },
        error: function (xhr, status, error) {
            console.error(`Error fetching data from ${url}:`, error);
            $(`#${sectionId}`).append(`<p>Error loading data.</p>`);
        }
    });
}

function BindDropdownMultiselect(apiUrl, dropdownId, authToken, valueField, textField, placeholder) {
    axios.get(apiUrl, {
        headers: {
            "Authorization": `Bearer ${authToken}`
        }
    })
        .then(function (response) {
            let dropdown = $(`#${dropdownId}`);

            if ($.fn.select2 && dropdown.hasClass('select2-hidden-accessible')) {
                dropdown.select2('destroy');
            }
            dropdown.empty();
            response.data.forEach((item, index) => {
                let rawValue = item[valueField];
                let rawText = item[textField];

                if (rawValue && rawText) {
                    let value = rawValue.toString();
                    let text = rawText.toString();
                    dropdown.append(`<option value="${value}">${text}</option>`);
                } else {
                    console.warn(`Skipping invalid item at index ${index}:`, item);
                }
            });
            dropdown.attr('multiple', 'multiple');
            dropdown.val(null);
            dropdown.select2({
                placeholder: `${placeholder}`,
                allowClear: true,
                width: '100%',
                dropdownParent: dropdown.parent(),
                selectOnClose: false
            });
        })
        .catch(function (error) {
            console.error(`Error fetching data for ${dropdownId}:`, error);
        });
}

function loadCheckboxesWithSearch(apiUrl, labelField, valueField, sectionId, className, payload) {
    let fullData = [];
    const selectedValues = new Set();

    $.ajax({
        url: apiUrl,
        method: 'POST',
        data: JSON.stringify(payload),
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            if (!Array.isArray(response)) {
                console.error("Expected array but got:", response);
                $(`#${sectionId}`).append('<p style="color:red;">Invalid data format.</p>');
                return;
            }

            fullData = response;
            renderSearchBox();
            renderCheckboxes(fullData);
        },
        error: function () {
            $(`#${sectionId}`).append('<p style="color:red;">Failed to load data.</p>');
        }
    });

    function renderSearchBox() {
        const $searchInput = $(`<input type="text" placeholder="Search..." style="margin-bottom:10px;width:100%; position: sticky; top: 0; background: white; z-index: 10;" class="form-control">`);
        $(`#${sectionId}`).empty().append($searchInput);

        $searchInput.on('keyup', function () {
            const searchTerm = $(this).val().toLowerCase();
            const filtered = fullData.filter(item => {
                const label = item[labelField];
                return label && label.toLowerCase().includes(searchTerm);
            });
            renderCheckboxes(filtered);
        });
    }

    function renderCheckboxes(items) {

        $(`#${sectionId}`).children('label, p').remove();

        const processedItems = items.map(item => ({
            value: item[valueField],
            label: item[labelField]
        })).filter(item => item.value !== undefined && item.label !== undefined);

        const checkedItems = [];
        const uncheckedItems = [];

        processedItems.forEach(item => {
            const isChecked = selectedValues.has(item.value);
            const $label = $('<label style="display:block;"></label>');
            const $checkbox = $(`<input type="checkbox" class="${className || ''}">`)
                .val(item.value)
                .prop('checked', isChecked);

            $label.append($checkbox).append(' ' + item.label);

            if (isChecked) {
                checkedItems.push({ label: item.label, $el: $label });
            } else {
                uncheckedItems.push({ label: item.label, $el: $label });
            }
        });

        checkedItems.sort((a, b) => a.label.localeCompare(b.label));
        uncheckedItems.sort((a, b) => a.label.localeCompare(b.label));

        [...checkedItems, ...uncheckedItems].forEach(({ $el }) => {
            $(`#${sectionId}`).append($el);
        });

        $(`#${sectionId} input[type="checkbox"]`).on('change', function () {
            const val = $(this).val();
            if (this.checked) {
                selectedValues.add(val);
            } else {
                selectedValues.delete(val);
            }

            renderCheckboxes(fullData);
        });
    }
}

function loadCheckboxesWithSearch2(apiUrl, labelField, valueField, sectionId, className) {
    let fullData = [];
    const selectedValues = new Set();
    let selectAllCheckbox = null;
    $.ajax({
        url: apiUrl,
        method: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            if (!Array.isArray(response)) {
                console.error("Expected array but got:", response);
                $(`#${sectionId}`).append('<p style="color:red;">Invalid data format.</p>');
                return;
            }
            fullData = response;
            renderSearchBox();
            renderSelectAllCheckbox();
            renderCheckboxes(fullData);
        },
        error: function () {
            $(`#${sectionId}`).append('<p style="color:red;">Failed to load data.</p>');
        }
    });
    function renderSearchBox() {
        const $searchInput = $(`<input type="text" placeholder="Search..." style="margin-bottom:10px;width:100%; position: sticky; top: 0; background: white; z-index: 10;" class="form-control">`);
        //  $(`#${sectionId}`).empty().append($searchInput);

        $searchInput.on('keyup', function () {
            const searchTerm = $(this).val().toLowerCase();
            const filtered = fullData.filter(item => {
                const label = item[labelField];
                return label && label.toLowerCase().includes(searchTerm);
            });
            renderCheckboxes(filtered);
            updateSelectAllState();
        });
    }
    function renderSelectAllCheckbox() {
        const $selectAllLabel = $('<label style="display:block; font-weight:bold; padding-bottom:5px; margin-bottom:10px;" class="text-dark"></label>');
        selectAllCheckbox = $(`<input type="checkbox" class="${className || ''}" id="selectAll_${sectionId}">`);
        $selectAllLabel.append(selectAllCheckbox).append(' Select All');
        $(`#${sectionId}`).append($selectAllLabel);

        selectAllCheckbox.on('change', function () {
            const isChecked = this.checked;
            if (isChecked) {
                fullData.forEach(item => {
                    selectedValues.add(item[valueField]);
                });
            } else {
                selectedValues.clear();
            }
            renderCheckboxes(fullData);
        });
    }
    function renderCheckboxes(items) {
        $(`#${sectionId}`).children('label').not(':first, :eq(1)').remove();
        $(`#${sectionId}`).children('p').remove();
        const processedItems = items.map(item => ({
            value: item[valueField],
            label: item[labelField]
        })).filter(item => item.value !== undefined && item.label !== undefined);
        const checkedItems = [];
        const uncheckedItems = [];
        processedItems.forEach(item => {
            const isChecked = selectedValues.has(item.value);
            const $label = $('<label style="display:block;" class="text-dark"></label>');
            const $checkbox = $(`<input type="checkbox" class="${className || ''}" data-individual="true">`)
                .val(item.value)
                .prop('checked', isChecked);
            $label.append($checkbox).append(' ' + item.label);
            if (isChecked) {
                checkedItems.push({ label: item.label, $el: $label });
            } else {
                uncheckedItems.push({ label: item.label, $el: $label });
            }
        });
        checkedItems.sort((a, b) => a.label.localeCompare(b.label));
        uncheckedItems.sort((a, b) => a.label.localeCompare(b.label));
        [...checkedItems, ...uncheckedItems].forEach(({ $el }) => {
            $(`#${sectionId}`).append($el);
        });
        $(`#${sectionId} input[type="checkbox"][data-individual="true"]`).on('change', function () {
            const val = $(this).val();
            if (this.checked) {
                selectedValues.add(val);
            } else {
                selectedValues.delete(val);
            }
            updateSelectAllState();
        });
        updateSelectAllState();
    }
    function updateSelectAllState() {
        if (selectAllCheckbox) {
            const totalItems = fullData.length;
            const selectedCount = selectedValues.size;
            if (selectedCount === 0) {
                selectAllCheckbox.prop('checked', false);
                selectAllCheckbox.prop('indeterminate', false);
            } else if (selectedCount === totalItems) {
                selectAllCheckbox.prop('checked', true);
                selectAllCheckbox.prop('indeterminate', false);
            } else {
                selectAllCheckbox.prop('checked', false);
                selectAllCheckbox.prop('indeterminate', true);
            }
        }
    }
    this.getSelectedValues = function () {
        return Array.from(selectedValues);
    };
    this.setSelectedValues = function (values) {
        selectedValues.clear();
        values.forEach(val => selectedValues.add(val));
        renderCheckboxes(fullData);
        updateSelectAllState();
    };
}

function loadCheckboxesWithSearchChecked(apiUrl, labelField, valueField, sectionId, className, preSelectedValues = []) {
    let fullData = [];
    const selectedValues = new Set(preSelectedValues);
    let selectAllCheckbox = null;
    $.ajax({
        url: apiUrl,
        method: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            if (!Array.isArray(response)) {
                console.error("Expected array but got:", response);
                $(`#${sectionId}`).append('<p style="color:red;">Invalid data format.</p>');
                return;
            }
            fullData = response;
            renderSearchBox();
            renderSelectAllCheckbox();
            renderCheckboxes(fullData);
        },
        error: function () {
            $(`#${sectionId}`).append('<p style="color:red;">Failed to load data.</p>');
        }
    });
    function renderSearchBox() {
        const $searchInput = $(`<input type="text" placeholder="Search..." style="margin-bottom:10px;width:100%; position: sticky; top: 0; background: white; z-index: 10;" class="form-control">`);
        // $(`#${sectionId}`).empty().append($searchInput);
        $searchInput.on('keyup', function () {
            const searchTerm = $(this).val().toLowerCase();
            const filtered = fullData.filter(item => {
                const label = item[labelField];
                return label && label.toLowerCase().includes(searchTerm);
            });
            renderCheckboxes(filtered);
            updateSelectAllState();
        });
    }
    function renderSelectAllCheckbox() {
        const $selectAllLabel = $('<label style="display:block; font-weight:bold; padding-bottom:5px; margin-bottom:10px;" class="text-dark"></label>');
        selectAllCheckbox = $(`<input type="checkbox" class="${className || ''}" id="selectAll_${sectionId}">`);
        $selectAllLabel.append(selectAllCheckbox).append(' Select All');
        $(`#${sectionId}`).append($selectAllLabel);
        selectAllCheckbox.on('change', function () {
            const isChecked = this.checked;
            if (isChecked) {
                fullData.forEach(item => {
                    selectedValues.add(item[valueField]);
                });
            } else {
                selectedValues.clear();
            }
            renderCheckboxes(fullData);
        });
    }
    function renderCheckboxes(items) {
        $(`#${sectionId}`).children('label').not(':first, :eq(1)').remove();
        $(`#${sectionId}`).children('p').remove();
        const processedItems = items.map(item => ({
            value: item[valueField],
            label: item[labelField]
        })).filter(item => item.value !== undefined && item.label !== undefined);
        const checkedItems = [];
        const uncheckedItems = [];
        processedItems.forEach(item => {
            const isChecked = selectedValues.has(item.value);
            const $label = $('<label style="display:block;" class="text-dark"></label>');
            const $checkbox = $(`<input type="checkbox" class="${className || ''}" data-individual="true">`)
                .val(item.value)
                .prop('checked', isChecked);
            $label.append($checkbox).append(' ' + item.label);
            if (isChecked) {
                checkedItems.push({ label: item.label, $el: $label });
            } else {
                uncheckedItems.push({ label: item.label, $el: $label });
            }
        });
        checkedItems.sort((a, b) => a.label.localeCompare(b.label));
        uncheckedItems.sort((a, b) => a.label.localeCompare(b.label));
        [...checkedItems, ...uncheckedItems].forEach(({ $el }) => {
            $(`#${sectionId}`).append($el);
        });
        $(`#${sectionId} input[type="checkbox"][data-individual="true"]`).on('change', function () {
            const val = $(this).val();
            if (this.checked) {
                selectedValues.add(val);
            } else {
                selectedValues.delete(val);
            }
            updateSelectAllState();
        });
        updateSelectAllState();
    }
    function updateSelectAllState() {
        if (selectAllCheckbox) {
            const totalItems = fullData.length;
            const selectedCount = selectedValues.size;
            if (selectedCount === 0) {
                selectAllCheckbox.prop('checked', false);
                selectAllCheckbox.prop('indeterminate', false);
            } else if (selectedCount === totalItems) {
                selectAllCheckbox.prop('checked', true);
                selectAllCheckbox.prop('indeterminate', false);
            } else {
                selectAllCheckbox.prop('checked', false);
                selectAllCheckbox.prop('indeterminate', true);
            }
        }
    }
    this.getSelectedValues = function () {
        return Array.from(selectedValues);
    };
    this.setSelectedValues = function (values) {
        selectedValues.clear();
        values.forEach(val => selectedValues.add(val));
        renderCheckboxes(fullData);
        updateSelectAllState();
    };
    this.addSelectedValues = function (values) {
        values.forEach(val => selectedValues.add(val));
        renderCheckboxes(fullData);
        updateSelectAllState();
    };
    this.removeSelectedValues = function (values) {
        values.forEach(val => selectedValues.delete(val));
        renderCheckboxes(fullData);
        updateSelectAllState();
    };
}

function toDateInputFormat(dateString) {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = (`0${date.getMonth() + 1}`).slice(-2);
    const day = (`0${date.getDate()}`).slice(-2);
    return `${year}-${month}-${day}`;
}

function fireSweetAlert(message, icon) {
    Swal.fire({
        text: `${message}`,
        icon: `${icon}`
    });
}
function createDynamicModal({ heading, buttons = [], apiUrl, PayLoad = {} }) {
    debugger
    const modalId = 'myDynamicModal';
    $(`#${modalId}`).remove();
    const modalTemplate = `
        <div class="modal fade" id="${modalId}" tabindex="-1" aria-labelledby="${modalId}Label" aria-hidden="true">
          <div class="modal-dialog modal-xl modal-dialog-scrollable">
            <div class="modal-content">
              <div class="modal-header bg-primary text-white">
                <h5 class="modal-title fw-bold" id="${modalId}Label">
                  <i class="fas fa-table me-2"></i>${heading}
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body p-0">
                <div class="table-responsive">
                  <div id="${modalId}Loading" class="text-center p-4">
                    <div class="spinner-border text-primary" role="status">
                      <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="mt-2 text-muted">Loading data...</p>
                  </div>
                  <table id="${modalId}Table" class="table table-hover table-striped mb-0" style="width:100%; display: none;">
                    <thead class="table-dark sticky-top"></thead>
                    <tbody></tbody>
                  </table>
                </div>
              </div>
              <div class="modal-footer bg-light">
                ${buttons.map((btn, i) => `
                  <button type="button" class="btn ${btn.class || 'btn-secondary'} ${btn.icon ? 'btn-icon' : ''}" id="${modalId}Btn${i}">
                    ${btn.icon ? `<i class="${btn.icon} me-1"></i>` : ''}${btn.text}
                  </button>
                `).join('')}
              </div>
            </div>
          </div>
        </div>
    `;

    $('body').append(modalTemplate);
    buttons.forEach((btn, i) => {
        $(`#${modalId}Btn${i}`).on('click', btn.onClick);
    });
    fetchAndPopulateTable(apiUrl, `${modalId}Table`, PayLoad);
    const modalInstance = new bootstrap.Modal(document.getElementById(modalId));
    modalInstance.show();
}

function fetchAndPopulateTable(apiUrl, tableId, PayLoad = {}, method = 'POST') {
    const loadingId = tableId.replace('Table', 'Loading');
    $(`#${loadingId}`).show();
    $(`#${tableId}`).hide();

    $.ajax({
        url: apiUrl,
        type: method,
        contentType: 'application/json',
        data: method === 'POST' ? JSON.stringify(PayLoad) : undefined,
        success: function (data) {
            $(`#${loadingId}`).hide();
            if (!data || data.length === 0) {
                $(`#${tableId}`).show();
                $(`#${tableId} thead`).html(`
                    <tr>
                        <th class="text-center py-4">
                            <i class="fas fa-inbox fa-2x text-muted mb-2 d-block"></i>
                            No Data Available
                        </th>
                    </tr>
                `);
                $(`#${tableId} tbody`).html(`
                    <tr>
                        <td class="text-center py-5 text-muted">
                            <p class="mb-0">No records found matching your criteria.</p>
                            <small>Try adjusting your search parameters or check back later.</small>
                        </td>
                    </tr>
                `);
                return;
            }
            const headers = Object.keys(data[0]);
            const theadHtml = `
                <tr>
                    ${headers.map(header => `
                        <th class="text-nowrap fw-semibold">
                            <div class="d-flex align-items-center">
                                <span>${formatHeaderName(header)}</span>
                                <i class="fas fa-sort ms-1 text-muted small"></i>
                            </div>
                        </th>
                    `).join('')}
                </tr>
            `;
            const tbodyHtml = data.map((row, index) => {
                const rowClass = index % 2 === 0 ? 'table-row-even' : 'table-row-odd';
                return `
                    <tr class="${rowClass}">
                        ${headers.map(header => `
                            <td class="align-middle">
                                <div class="cell-content">
                                    ${formatCellValue(row[header], header)}
                                </div>
                            </td>
                        `).join('')}
                    </tr>
                `;
            }).join('');
            $(`#${tableId} thead`).html(theadHtml);
            $(`#${tableId} tbody`).html(tbodyHtml);
            $(`#${tableId}`).show();
            if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
                $(`#${tableId}`).DataTable().clear().destroy();
            }
            $(`#${tableId}`).DataTable({
                scrollX: true,
                autoWidth: false,
                responsive: true,
                pageLength: 25,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                    '<"row"<"col-sm-12"tr>>' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                language: {
                    search: "_INPUT_",
                    searchPlaceholder: "Search records...",
                    lengthMenu: "Show _MENU_ entries",
                    info: "Showing _START_ to _END_ of _TOTAL_ entries",
                    infoEmpty: "Showing 0 to 0 of 0 entries",
                    infoFiltered: "(filtered from _MAX_ total entries)",
                    paginate: {
                        first: "First",
                        last: "Last",
                        next: "Next",
                        previous: "Previous"
                    }
                },
                columnDefs: [
                    {
                        targets: '_all',
                        className: 'text-nowrap'
                    }
                ],
                order: [],
                drawCallback: function (settings) {
                    $(`#${tableId} tbody tr`).hover(
                        function () { $(this).addClass('table-hover-effect'); },
                        function () { $(this).removeClass('table-hover-effect'); }
                    );
                }
            });
            addCustomTableStyles(tableId);
        },
        error: function (xhr, status, error) {
            console.error("API error:", error);
            $(`#${loadingId}`).hide();
            $(`#${tableId}`).show();
            $(`#${tableId} thead`).html(`
                <tr>
                    <th class="text-center py-4 bg-danger text-white">
                        <i class="fas fa-exclamation-triangle fa-2x mb-2 d-block"></i>
                        Error Loading Data
                    </th>
                </tr>
            `);
            $(`#${tableId} tbody`).html(`
                <tr>
                    <td class="text-center py-5">
                        <div class="alert alert-danger mb-0">
                            <h6 class="alert-heading">
                                <i class="fas fa-times-circle me-2"></i>Failed to Load Data
                            </h6>
                            <p class="mb-2"><strong>Error:</strong> ${error}</p>
                            <small class="text-muted">
                                Status: ${xhr.status} - ${xhr.statusText || status}
                            </small>
                            <hr>
                            <button class="btn btn-outline-danger btn-sm" onclick="location.reload()">
                                <i class="fas fa-redo me-1"></i>Retry
                            </button>
                        </div>
                    </td>
                </tr>
            `);
        }
    });
}

// Helper function to format header names
function formatHeaderName(header) {
    return header
        .replace(/([A-Z])/g, ' $1')
        .replace(/^./, str => str.toUpperCase())
        .trim();
}

// Helper function to format cell values based on data type
function formatCellValue(value, header) {
    if (value === null || value === undefined || value === '') {
        return '<span class="text-muted fst-italic">N/A</span>';
    }

    // Format different data types
    if (typeof value === 'boolean') {
        return value
            ? '<span class="badge bg-success"><i class="fas fa-check me-1"></i>Yes</span>'
            : '<span class="badge bg-secondary"><i class="fas fa-times me-1"></i>No</span>';
    }

    if (typeof value === 'number') {
        // Check if it's a date timestamp
        if (header.toLowerCase().includes('date') || header.toLowerCase().includes('time')) {
            return new Date(value).toLocaleDateString();
        }
        // Format numbers with commas
        return value.toLocaleString();
    }

    if (typeof value === 'string') {
        // Check for email
        if (value.includes('@') && value.includes('.')) {
            return `<a href="mailto:${value}" class="text-decoration-none">${value}</a>`;
        }

        // Check for URLs
        if (value.startsWith('http')) {
            return `<a href="${value}" target="_blank" class="text-decoration-none">
                        ${value.length > 30 ? value.substring(0, 30) + '...' : value}
                        <i class="fas fa-external-link-alt ms-1 small"></i>
                    </a>`;
        }

        // Truncate long text
        if (value.length > 50) {
            return `<span title="${value}" data-bs-toggle="tooltip">
                        ${value.substring(0, 50)}...
                    </span>`;
        }
    }

    return value;
}

// Function to add custom CSS styles
function addCustomTableStyles(tableId) {
    if (!$('#dynamicTableStyles').length) {
        $('<style id="dynamicTableStyles">').html(`
            .table-hover-effect {
                background-color: rgba(0, 123, 255, 0.1) !important;
                transform: scale(1.01);
                transition: all 0.2s ease;
                box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            }
            
            .cell-content {
                max-width: 200px;
                overflow: hidden;
                text-overflow: ellipsis;
            }
            
            .table thead th {
                border-bottom: 2px solid #dee2e6;
                background: linear-gradient(135deg, #343a40 0%, #495057 100%);
                color: white;
                font-weight: 600;
                text-transform: uppercase;
                font-size: 0.85rem;
                letter-spacing: 0.5px;
            }
            
            .table tbody tr:hover {
                background-color: rgba(0, 123, 255, 0.05);
            }
            
            .dataTables_wrapper .dataTables_length select,
            .dataTables_wrapper .dataTables_filter input {
                border: 1px solid #ced4da;
                border-radius: 0.375rem;
                padding: 0.375rem 0.75rem;
            }
            
            .dataTables_wrapper .dataTables_filter input:focus {
                border-color: #86b7fe;
                outline: 0;
                box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
            }
            
            .modal-xl {
                max-width: 95%;
            }
            
            @media (max-width: 768px) {
                .modal-xl {
                    max-width: 100%;
                    margin: 0;
                }
                
                .cell-content {
                    max-width: 120px;
                }
            }
        `).appendTo('head');
    }

    // Initialize tooltips
    setTimeout(() => {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }, 100);
}
