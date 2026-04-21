$(document).on('click', '[data-pagination-link]', function (e) {
    e.preventDefault();

    var $el = $(this);

    var page = $el.data('page');
    var pageSize = $el.data('pagesize');
    var baseUrl = $el.data('baseurl');
    var reloadFunction = $el.data('reloadfunction');

    // Lấy query string hiện tại từ URL
    var params = new URLSearchParams(window.location.search);

    // Cập nhật lại page và pageSize
    params.set('page', page);
    params.set('pageSize', pageSize);

    var fullUrl = baseUrl + '?' + params.toString();

    // Gọi hàm reload ajax
    if (typeof window[reloadFunction] === 'function') {
        window[reloadFunction](fullUrl);
    } else {
        console.warn('Reload function "' + reloadFunction + '" not found');
    }
});

$(document).ready(function () {
    $('.table-container').each(function () {
        const $table = $(this).find('table');
        const columnCount = $table.find('thead tr th').length;

        if (columnCount >= 4) {
            $(this).addClass('scrollable');
        } else {
            $(this).removeClass('scrollable'); // đảm bảo không giữ class nếu ít cột
        }
    });
    $('.data-table tbody tr').click(function () {
        // Bỏ chọn tất cả dòng trước
        $('.data-table tbody tr').removeClass('selected');
        // Thêm class cho dòng được click
        $(this).addClass('selected');
    });
});
function initSelect2(selector) {
    $(document).ready(function () {
        $(selector).select2({
            allowClear: true,
            width: '100%'
        });
    });
}
function setupDeleteButton(btnSelector, rowSelector, getIdFunc, deleteUrlBase, reloadUrl) {
    $(document).on('click', btnSelector, function () {
        const $selectedRow = $(`${rowSelector}.selected`);
        if ($selectedRow.length === 0) {
            alert("Vui lòng chọn dòng để xóa!");
            return;
        }

        const id = getIdFunc($selectedRow);
        if (!id) {
            alert("Dữ liệu không hợp lệ!");
            return;
        }

        if (!confirm("Bạn có chắc muốn xóa?")) {
            return;
        }

        $.ajax({
            url: `${deleteUrlBase}${id}`,
            type: "DELETE",
            success: () => reloadAdminTable(reloadUrl),
            error: () => alert("Lỗi khi xóa!")
        });
    });
}

$(function () {
    // Click vào dòng để chọn
    $(document).on("click", ".data-row", function () {
        $(".data-row").removeClass("selected");
        $(this).addClass("selected");
    });

    $(document).on("click", ".btn-open-modal", function () {
        let url = $(this).data("url");
        let idsrc = $(this).data("idsrc");
        const title = $(this).data("title") || "Thông tin";
        const target = $(this).data("target");
        if (!url || !target) {
            alert("Thiếu URL hoặc target.");
            return;
        }
        // Nếu là cập nhật, lấy ID từ dòng đã chọn
        if (title && title.toLowerCase().includes("cập nhật")) {
            const $selected = $(".data-row.selected");
            if ($selected.length === 0) {
                alert("Vui lòng chọn dòng để cập nhật.");
                return;
            }

            const id = $selected.data("id");
            url += `?id=${id}`;
        }
        if (idsrc)
        {
            url += `?idsrc=${idsrc}`;
        }
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                const $modal = $(target);
                $modal.find(".custom-modal-title").text(title);
                $modal.find(".custom-modal-body").html(data);
                $modal.show();
            },
            error: function () {
                alert("Không thể tải nội dung modal.");
            }
        });
    });

    $(document).on("click", ".custom-modal-close", function () {
        const target = $(this).data("target");
        $(target).hide();
        $(target).find(".custom-modal-body").html("");
    });
});

function reloadAdminTable(baseUrl) {
    if (!baseUrl) {
        alert("Chưa có URL để tải lại bảng dữ liệu.");
        return;
    }

    // Lấy query string hiện tại từ URL (VD: ?name=abc&page=2)
    var queryParams = window.location.search;
    var fullUrl = baseUrl + queryParams;
    $.ajax({
        url: fullUrl,
        type: "GET",
        success: function (data) {
            $("#dataTableContainer").html(data);
        },
        error: function () {
            alert("Không thể tải lại bảng dữ liệu.");
        }
    });
}

$(function () {
    var isOpen = true;
    $("#logo").click(function () {
        if (isOpen) {
            $("#navMainMenu").addClass("hide-menu");
            $("#adminBody").addClass("hide-menu");
            isOpen = false;
        } else {
            $("#adminBody").removeClass("hide-menu");
            $("#navMainMenu").removeClass("hide-menu");
            isOpen = true;
        }
        //FW.setSession('OpenAdminMenu', isOpen ? '1' : '0');
        $(window).resize();
    })

    //if ('0' == FW.getSession('OpenAdminMenu')) {
    //    $("#navMainMenu").addClass("hide-menu");
    //    $("#adminBody").addClass("hide-menu");
    //    isOpen = false;
    //}

    if ($(window).width() <= 820) {
        $("#navMainMenu").addClass("hide-menu");
        $("#adminBody").addClass("hide-menu");
        isOpen = false;
    }

    $('#navMainMenu>.select').click(function () {
        var index = $('#navMainMenu>.select').index(this);
        $('#navMainMenu>.select').removeClass("current");
        $($('#navMainMenu>.select').get(index)).addClass("current");
    })

    $(".btn-guide-console").click(function () {
        $(".admin-menu-page").actionComponent("ShowContentMenu", { data: $(this).data().value });
    })

    $("body").on("click", ".btn-refresh-search", function () {
        $(this).closest(".search-form").resetForm();
        $(this).actionComponent("ReloadPaging");
    })

    $("body").on("click", ".btn-menu-item-action", function () {
        var $parent = $(this).closest(".menu-item-action");
        var $menuList = $parent.find(".menu-item-list");
        $parent.addClass("open");
        $menuList.position({
            of: $parent,
            my: 'left top',
            at: 'left bottom',
            collision: "flipfit"
        });

    }).on("click", ".menu-item-close", function () {
        $(this).closest(".menu-item-action").removeClass("open");
    }).on("click", ".menu-item-action .action-btn", function () {
        $(this).closest(".menu-item-action").removeClass("open");
    });

    //FilterMenu
    $("body").on("click", ".btn-filter-menu", function () {
        var $parent = $(this).closest(".filter-menu");
        $parent.addClass("open");
        var $menuList = $parent.find(".search-form");
        $menuList.position({
            of: $parent,
            my: 'left top',
            at: 'left bottom'
        });
    });
    $("body").on("click", ".filter-menu .bg-close", function () {
        var $parent = $(this).closest(".filter-menu");
        $parent.removeClass("open");
    });
})

function InitListEvent(listid, autoHeight) {
    var lstMain = $("#" + listid);
    lstMain.addClass("list-main");
    if (autoHeight) {
        var wrapList = $("#adminBody");
        $(window).resize(function () {
            var h = wrapList.outerHeight();
            var t = h - lstMain.position().top - 10;
            if (lstMain.find(".paging-bottom:visible").size() > 0) {
                t -= lstMain.find(".paging-bottom").outerHeight();
            }
            if (lstMain.find(".paging-top:visible").size() > 0) {
                t -= lstMain.find(".paging-top").outerHeight();
            }
            lstMain.children(".wrap-grid-content").css("height", t);
        })
        setTimeout(function () {
            $(window).resize();
        }, 200);
    }
    if ($.isMobile) {
        lstMain.addClass("grid-reponsive");
    }
}

//Grid Reponsive
$(function () {
    if ($.isMobile) {
        var $rowDetail = null;
        $("body").on("click", ".grid-reponsive .row-field", function (evt) {
            var $lstField = $(this).find(">td.field");
            var $lstFieldVisible = $(this).find(">td.field:visible");
            if (!$rowDetail) {
                var countField = $lstFieldVisible.size();
                $rowDetail = $("<tr class='row-reponsive'><td colspan='" + countField + "'><div class='grid-wrap-item-reponsive'></div></td></tr>");
            }
            if (!$(this).hasClass("open-detail")) {
                var $gridReponsive = $(this).closest(".grid-reponsive");
                $gridReponsive.find(".open-detail").removeClass("open-detail");
                var $wrapScroll = $gridReponsive.find(".wrap-grid-content");
                $(this).addClass("open-detail");
                $rowDetail.setDisplay(true);
                $rowDetail.attr("data", $(this).attr("data"));
                $rowDetail.insertAfter($(this));
                var $cell = $rowDetail.find(".grid-wrap-item-reponsive");
                var render = '';
                $lstField.each(function (i, e) {
                    var title = $gridReponsive.find(".grid-header .field-index-" + i).text();
                    render += ("<div class='grid-reponsive-item'><div class='item-name'>" + title + "</div><div class='item-value'>" + $(e).find(">.field-value").html() + "</div></div>");
                });
                $cell.html(render);
                var $btnAction = $cell.find(".grid-item-action");
                $cell.append($btnAction);
                $wrapScroll.stop().animate({ scrollTop: $(this).position().top }, 200, 'swing');
            } else {
                $(this).removeClass("open-detail");
                $rowDetail.setDisplay(false);
            }
        });
    }
})