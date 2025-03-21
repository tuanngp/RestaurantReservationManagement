function initPagination(tableSelector) {
    const table = $(tableSelector);
    const tbody = table.find('tbody');
    const rows = tbody.find('tr').toArray();
    let currentPage = 1;
    let itemsPerPage = 5;
    const maxVisiblePages = 5;

    if (rows.length === 0 || (rows.length === 1 && rows[0].querySelector('.no-data-message'))) {
        table.addClass('no-data');
        return;
    } else {
        table.removeClass('no-data');
    }

    if (!table.next('#pagination-container').length && rows.length > 0) {
        table.after(`
            <div id="pagination-container" class="card-footer clearfix">
                <div class="d-flex align-items-center">
                    <select id="items-per-page" class="form-control">
                        <option value="5">5</option>
                        <option value="10">10</option>
                        <option value="25">25</option>
                        <option value="50">50</option>
                    </select>
                    <span class="pagination-info"></span>
                </div>
                <ul class="pagination pagination-sm m-0"></ul>
            </div>
        `);
    }

    const paginationContainer = $('#pagination-container .pagination');
    const itemsPerPageSelect = $('#items-per-page');
    const paginationInfo = $('#pagination-container .pagination-info');

    function showPage(page, animate = true) {
        const start = (page - 1) * itemsPerPage;
        const end = start + itemsPerPage;
        const displayedRows = rows.slice(start, end);

        tbody.find('tr').hide();

        if (animate) {
            displayedRows.forEach((row, index) => {
                $(row).fadeIn(200 + index * 50).css('display', 'table-row');
                $(row).find('td:first').text(start + index + 1);
            });
        } else {
            displayedRows.forEach((row, index) => {
                $(row).show();
                $(row).find('td:first').text(start + index + 1);
            });
        }

        currentPage = page;
        updatePagination();
        updateInfo();
    }

    function updateInfo() {
        const totalItems = rows.length;
        const start = (currentPage - 1) * itemsPerPage + 1;
        const end = Math.min(currentPage * itemsPerPage, totalItems);
        const totalPages = Math.ceil(totalItems / itemsPerPage);
        paginationInfo.text(`Hiển thị ${start}-${end} của ${totalItems} mục (Trang ${currentPage}/${totalPages})`);
    }

    function updatePagination() {
        const totalPages = Math.ceil(rows.length / itemsPerPage);
        paginationContainer.empty();

        // Nút Previous
        if (totalPages > 1) {
            const prevLi = $('<li class="page-item">').toggleClass('disabled', currentPage === 1);
            const prevLink = $('<a class="page-link" href="#" aria-label="Previous">')
                .html('<i class="fas fa-chevron-left"></i>');
            prevLi.append(prevLink);
            paginationContainer.append(prevLi);

            prevLink.on('click', function(e) {
                e.preventDefault();
                if (currentPage > 1) {
                    showPage(currentPage - 1);
                }
            });
        }

        // Tính toán phạm vi trang hiển thị
        let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
        let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
        
        if (endPage - startPage + 1 < maxVisiblePages) {
            startPage = Math.max(1, endPage - maxVisiblePages + 1);
        }

        // Nút trang đầu và dấu ...
        if (startPage > 1) {
            paginationContainer.append(
                $('<li class="page-item">')
                    .append($('<a class="page-link" href="#">').text('1'))
                    .on('click', () => showPage(1))
            );
            if (startPage > 2) {
                paginationContainer.append(
                    $('<li class="page-item disabled">')
                        .append($('<a class="page-link" href="#">').text('...'))
                );
            }
        }

        // Các nút số trang
        for (let i = startPage; i <= endPage; i++) {
            const pageLi = $('<li class="page-item">').toggleClass('active', i === currentPage);
            const pageLink = $('<a class="page-link" href="#">').text(i);
            pageLi.append(pageLink);
            paginationContainer.append(pageLi);

            pageLink.on('click', function(e) {
                e.preventDefault();
                showPage(i);
            });
        }

        // Dấu ... và nút trang cuối
        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                paginationContainer.append(
                    $('<li class="page-item disabled">')
                        .append($('<a class="page-link" href="#">').text('...'))
                );
            }
            paginationContainer.append(
                $('<li class="page-item">')
                    .append($('<a class="page-link" href="#">').text(totalPages))
                    .on('click', () => showPage(totalPages))
            );
        }

        // Nút Next
        if (totalPages > 1) {
            const nextLi = $('<li class="page-item">').toggleClass('disabled', currentPage === totalPages);
            const nextLink = $('<a class="page-link" href="#" aria-label="Next">')
                .html('<i class="fas fa-chevron-right"></i>');
            nextLi.append(nextLink);
            paginationContainer.append(nextLi);

            nextLink.on('click', function(e) {
                e.preventDefault();
                if (currentPage < totalPages) {
                    showPage(currentPage + 1);
                }
            });
        }
    }

    // Xử lý sự kiện thay đổi số items/trang
    itemsPerPageSelect.on('change', function() {
        itemsPerPage = parseInt($(this).val());
        showPage(1, false);
    });

    // Khởi tạo hiển thị trang đầu tiên
    showPage(1, false);
}
