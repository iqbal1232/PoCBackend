node {
    checkout scm

    docker.withRegistry('https://registry.hub.docker.com', 'dockercre') {

        def customImage = docker.build("poc:nepoc")

        /* Push the container to the custom Registry */
        customImage.push()
    }
}
