﻿﻿﻿﻿﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function() {
    // Toggle Sidebar
    $('#sidebarCollapse').on('click', function() {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Add active class to current nav item
    const currentUrl = window.location.pathname;
    $('#sidebar .nav-link').each(function() {
        const href = $(this).attr('href');
        if (href && currentUrl.includes(href)) {
            $(this).addClass('active');
            // Expand parent if in dropdown
            $(this).parents('.collapse').addClass('show');
        }
    });

    // Form submission handling
    $('form').on('submit', function() {
        const submitBtn = $(this).find('button[type="submit"]');
        const spinner = submitBtn.find('.spinner-border');
        submitBtn.prop('disabled', true);
        spinner.removeClass('d-none');
        
        // Add loading overlay
        const card = $(this).closest('.card');
        if (card.length) {
            card.append('<div class="loading-overlay"><div class="spinner-border text-primary"></div></div>');
        }
    });

    // Animate form fields on focus
    $('.form-control').on('focus', function() {
        $(this).closest('.form-group').addClass('focused');
    }).on('blur', function() {
        if (!$(this).val()) {
            $(this).closest('.form-group').removeClass('focused');
        }
    });

    // Initialize tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(el => new bootstrap.Tooltip(el));

    // Initialize popovers
    const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');
    popoverTriggerList.forEach(el => new bootstrap.Popover(el));

    // Add hover effects to cards
    $('.card').hover(
        function() { 
            $(this).addClass('shadow-lg');
            $(this).find('.card-hover-show').fadeIn();
        },
        function() { 
            $(this).removeClass('shadow-lg');
            $(this).find('.card-hover-show').fadeOut();
        }
    );

    // Add animation to alerts
    $('.alert').addClass('animate__animated animate__fadeIn')
        .append('<button type="button" class="btn-close" data-bs-dismiss="alert"></button>');

    // Handle sidebar responsiveness
    function checkWidth() {
        if ($(window).width() <= 768) {
            $('#sidebar').addClass('active');
            $('#content').addClass('active');
        } else {
            $('#sidebar').removeClass('active');
            $('#content').removeClass('active');
        }
    }

    // Check width on load and resize
    checkWidth();
    $(window).resize(checkWidth);

    // Smooth scroll
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add fade animation to page transitions
    $(document).on('click', 'a:not([href^="#"])', function(e) {
        if ($(this).attr('target') !== '_blank' && !$(this).hasClass('no-transition')) {
            e.preventDefault();
            const href = $(this).attr('href');
            $('.content-wrapper').addClass('animate__animated animate__fadeOut');
            setTimeout(() => {
                window.location = href;
            }, 200);
        }
    });

    // Khởi tạo phân trang cho bảng dữ liệu
    if ($('#data-table').length) {
        initPagination('#data-table');
    }

    // Cập nhật phân trang khi tìm kiếm
    $('form[asp-action="Search"]').on('keyup', 'input[name="searchText"]', function() {
        const searchText = $(this).val().toLowerCase();
        const table = $('#data-table');
        const rows = table.find('tbody tr').toArray();
        
        rows.forEach(row => {
            const text = $(row).text().toLowerCase();
            $(row).toggle(text.includes(searchText));
        });

        // Cập nhật lại phân trang sau khi lọc
        if (typeof initPagination === 'function') {
            initPagination('#data-table');
        }
    });
});
