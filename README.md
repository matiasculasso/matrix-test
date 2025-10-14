Key points

    Use IndexOf instead of Replace or regex to scan lines — far less allocation and CPU.

    Deduplicate and sanitize the input word list so each unique word is searched exactly once.

    Precompute the transposed matrix to reuse the same horizontal search over columns efficiently.
