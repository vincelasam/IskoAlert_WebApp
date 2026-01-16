<script>
        // Optional: Auto-submit on Enter key
    document.addEventListener('DOMContentLoaded', function() {
            const searchInput = document.querySelector('input[name="keyword"]');
    if (searchInput) {
        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.form.submit();
            }
        });
            }
        });
</script>