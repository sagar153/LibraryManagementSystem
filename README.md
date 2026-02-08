# LibraryManagementSystem
.Net Core RESTFUL WEB API for library management system

# Setup Instructions
1. Clone the repo
2. Update the Nuget Packages
3. Create SQL table and dummy data
4. Run the application

# Key Functionality 
1. JWT Aunthentication
2. Handles Book Loan/Issue use cases
3. Handles Book reservation if books are not available 
4. Handles Members
 
# Folder Structure
1. Controllers - Contains all REST End Points
2. Data - EF Db Context - Database First Approach
3. Middlewares - Inspect/ Modify Incoming requests and response
4. Models - Contains DTO and request and response templates
5. Repository - Contains tools to Conect With Database
6. Services- Contains service layer to handle user requests

# Application Roles
1. User
2. Librarian
3. Admin

# Controllers

1. AuthController - Handles JWT Authentication and user role management for Library System

    Method	Route	                    Auth Required	Description
    POST	/api/v1/Auth/register	    No	            Register a new user with username, email, password, full name
    POST	/api/v1/Auth/login	        No	            Login with username/email and password, returns JWT access + refresh tokens
    POST	/api/v1/Auth/refresh	    No	            Exchange a refresh token for a new access token
    POST	/api/v1/Auth/logout	        No	            Revoke a refresh token to invalidate the session
    PUT	    /api/v1/Auth/assign-role    Admin only	    Assign User, Librarian, or Admin role to a user

2. BookLoansController  - Handles Books loan(Issue) functionalty, book checkout, return, renewal, and late fee operations. All endpoints require authentication

    Method	Route	                                    Description
    GET	    /api/v1/BookLoans	                        Get all loans
    GET	    /api/v1/BookLoans/{id}	                    Get a specific loan by ID
    GET	    /api/v1/BookLoans/member/{memberId}	        Get all loans for a member
    GET	    /api/v1/BookLoans/overdue	                Get all overdue loans
    GET	    /api/v1/BookLoans/member/{memberId}/active	Get active loans for a member
    POST	/api/v1/BookLoans	                        Checkout a book (creates a loan)
    POST	/api/v1/BookLoans/{id}/return	            Return a borrowed book
    POST	/api/v1/BookLoans/{id}/renew	            Renew/extend a loan
    POST	/api/v1/BookLoans/process/late-fees	        Calculate late fees for all overdue loans

3. BookReservationsController - Handles book reservation, set queue list if no books are in library

    Method	Route	                                    Description
    GET	    /api/v1/BookReservations	                Get all reservations
    GET	    /api/v1/BookReservations/{id}	            Get a specific reservation by ID
    GET	    /api/v1/BookReservations/member/{memberId}	Get all reservations for a member
    GET	    /api/v1/BookReservations/book/{bookId}	    Get all reservations for a book
    GET	    /api/v1/BookReservations/status/pending	    Get all pending reservations
    POST	/api/v1/BookReservations	                Create a new reservation
    PATCH	/api/v1/BookReservations/{id}/status	    Update reservation status
    POST	/api/v1/BookReservations/{id}/cancel	    Cancel a reservation
    POST	/api/v1/BookReservations/process/expired	Batch process all expired reservations

4. BooksController - Handles the books CRUD opertaion, add, Update and delete a book can be performed by librarian

    Method	    Route	                            Roles	                Description
    GET	        /api/v1/Books	                    User, Librarian, Admin	Get all books
    GET	        /api/v1/Books/{id}	                User, Librarian, Admin	Get a specific book by ID
    GET	        /api/v1/Books/category/{category}	User, Librarian, Admin	Filter books by category
    GET	        /api/v1/Books/search?searchTerm=	User, Librarian, Admin	Search by title, author, or ISBN
    POST	    /api/v1/Books	                    Admin, Librarian	    Add a new book
    PUT	        /api/v1/Books/{id}	                Admin, Librarian	    Update an existing book
    DELETE	    /api/v1/Books/{id}	                Admin, Librarian	    Soft delete a book (IsActive = false)

5. MembersController - Handle library members CRUD opetion, can only be performed by Admin role

    Method	Route	                                    Description
    GET	    /api/v1/Members	                            Get all members
    GET	    /api/v1/Members/{id}	                    Get a member by ID
    GET	    /api/v1/Members/membership/{membershipId}	Get a member by membership ID (e.g., LIB1001)
    GET	    /api/v1/Members/search?searchTerm=	        Search members by name or email
    POST	/api/v1/Members	                            Create a new member
    PUT	    /api/v1/Members/{id}	                    Update an existing member
    PATCH	/api/v1/Members/{id}/deactivate	            Deactivate a member (IsActive = false)
    DELETE	/api/v1/Members/{id}	                    Delete a member






